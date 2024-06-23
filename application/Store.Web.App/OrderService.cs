using Microsoft.AspNetCore.Http;
using PhoneNumbers;
using Store.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Store.Web.App
{
    public class OrderService
    {
        private readonly IComponentRepository componentRepository;
        private readonly IOrderRepository orderRepository;
        private readonly INotificationService notificationService;
        private readonly IHttpContextAccessor httpContextAccessor;

        protected ISession Session => httpContextAccessor.HttpContext.Session;

        public OrderService(IComponentRepository componentRepository,
                            IOrderRepository orderRepository,
                            INotificationService notificationService,
                            IHttpContextAccessor httpContextAccessor)
        {
            this.componentRepository = componentRepository;
            this.orderRepository = orderRepository;
            this.notificationService = notificationService;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<(bool hasValue, OrderModel model)> TryGetModelAsync()
        {
            var (hasValue, order) = await TryGetOrderAsync();
            if (hasValue)
                return (true, await MapAsync(order));

            return (false, null);
        }

        internal async Task<(bool hasValue, Order order)> TryGetOrderAsync()
        {
            if (Session.TryGetCart(out Cart cart))
            {
                var order = await orderRepository.GetByIdAsync(cart.OrderId);
                return (true, order);
            }

            return (false, null);
        }

        internal async Task<OrderModel> MapAsync(Order order)
        {
            var components = await GetComponentAsync(order);
            var items = from item in order.Items
                        join component in components on item.ComponentId equals component.Id
                        select new OrderItemModel
                        {
                            ComponentId = component.Id,
                            NameOfComponent = component.NameOfComponent,
                            Package = component.Package,
                            Price = item.Price,
                            Count = item.Count,
                        };

            return new OrderModel
            {
                Id = order.Id,
                Items = items.ToArray(),
                TotalCount = order.TotalCount,
                TotalPrice = order.TotalPrice,
                CellPhone = order.CellPhone,
                DeliveryDescription = order.Delivery?.Description,
                PaymentDescription = order.Payment?.Description
            };
        }

        internal async Task<IEnumerable<Component>> GetComponentAsync(Order order)
        {
            var componentIds = order.Items.Select(item => item.ComponentId);

            return await componentRepository.GetAllByIdsAsync(componentIds);
        }

        public async Task<OrderModel> AddComponentAsync(int componentId, int count)
        {
            if (count < 1)
                throw new InvalidOperationException("Too few components to add");

            var (hasValue, order) = await TryGetOrderAsync();

            if (!hasValue)
                order = await orderRepository.CreateAsync();

            await AddOrUpdateComponentAsync(order, componentId, count);
            UpdateSession(order);

            
            return await MapAsync(order);
        }

        internal async Task AddOrUpdateComponentAsync(Order order, int componentId, int count)
        {
            var component = await componentRepository.GetByIdAsync(componentId);
            if (order.Items.TryGet(componentId, out OrderItem orderItem))
                orderItem.Count += count;
            else
                order.Items.Add(component.Id, component.Price, count);
            await orderRepository.UpdateAsync(order);
        }

        internal void UpdateSession(Order order)
        {
            var cart = new Cart(order.Id, order.TotalCount, order.TotalPrice);
            Session.Set(cart);
        }

        public async Task<OrderModel> UpdateComponentAsync(int componentId, int count)
        {
            var order = await GetOrderAsync();
            order.Items.Get(componentId).Count = count;

            await orderRepository.UpdateAsync(order);
            UpdateSession(order);

            return await MapAsync(order);
        }

        public async Task<OrderModel> RemoveComponentAsync(int componentId)
        {
            var order = await GetOrderAsync();
            order.Items.Remove(componentId);

            await orderRepository.UpdateAsync(order);
            UpdateSession(order);

            return await MapAsync(order);
        }

        public async Task<Order> GetOrderAsync()
        {
            var (hasValue, order) = await TryGetOrderAsync();

            if (hasValue)
                return order;

            throw new InvalidOperationException("Empty session.");
        }
        public async Task<OrderModel> SendConfirmationAsync(string cellPhone)
        {
            var order = await GetOrderAsync();
            var model = await MapAsync(order);

            if (TryFormatPhone(cellPhone, out string formattedPhone))
            {
                var confirmationCode = 1111; // todo: random.Next(1000, 10000) = 1000, 1001, ..., 9998, 9999
                model.CellPhone = formattedPhone;
                Session.SetInt32(formattedPhone, confirmationCode);
                await notificationService.SendConfirmationCodeAsync(formattedPhone, confirmationCode);
            }
            else
                model.Errors["cellPhone"] = "Номер телефона не соответствует формату +79876543210";

            return model;
        }

        private readonly PhoneNumberUtil phoneNumberUtil = PhoneNumberUtil.GetInstance();

        internal bool TryFormatPhone(string cellPhone, out string formattedPhone)
        {
            try
            {
                var phoneNumber = phoneNumberUtil.Parse(cellPhone, "ru");
                formattedPhone = phoneNumberUtil.Format(phoneNumber, PhoneNumberFormat.INTERNATIONAL);
                return true;
            }
            catch (NumberParseException)
            {
                formattedPhone = null;
                return false;
            }
        }

        public async Task<OrderModel> ConfirmCellPhoneAsync(string cellPhone, int confirmationCode)
        {
            int? storedCode = Session.GetInt32(cellPhone);
            var model = new OrderModel();
            if (storedCode == null)
            {
                model.Errors["cellPhone"] = "Что-то случилось. Попробуйте получить код ещё разs.";
                return model;
            }
            if (storedCode != confirmationCode)
            {
                model.Errors["confirmationCode"] = "Неверный код. Проверьте и попробуйте ещё раз.";
                return model;
            }
                var order = await GetOrderAsync();
                order.CellPhone = cellPhone;
                await orderRepository.UpdateAsync(order);

                Session.Remove(cellPhone);

                return await MapAsync(order);
        }
        

            public async Task<OrderModel> SetDeliveryAsync(OrderDelivery delivery)
        {
            var order = await GetOrderAsync();
            order.Delivery = delivery;
            await orderRepository.UpdateAsync(order);

            return await MapAsync(order);
        }

        public async Task<OrderModel> SetPaymentAsync(OrderPayment payment)
        {
            var order = await GetOrderAsync();
            order.Payment = payment;
            await orderRepository.UpdateAsync(order);
            Session.RemoveCart();

            await notificationService.StartProcessAsync(order);

            return await MapAsync(order);
        }
    }
}