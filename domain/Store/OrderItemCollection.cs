﻿using Store.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Store
{
    public class OrderItemCollection : IReadOnlyCollection<OrderItem>
    {
        private readonly OrderDto orderDto;
        private readonly List<OrderItem> items;

        public OrderItemCollection(OrderDto orderDto)
        {
            if (orderDto == null)
                throw new ArgumentNullException(nameof(orderDto));

            this.orderDto = orderDto;

            items = orderDto.Items
                            .Select(OrderItem.Mapper.Map)
                            .ToList();
        }

        public int Count => items.Count;
        public IEnumerator<OrderItem> GetEnumerator()
        {
            return items.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (items as IEnumerable).GetEnumerator();
        }
        public OrderItem Get(int componentId)
        {
            if (TryGet(componentId, out OrderItem orderItem))
                return orderItem;
            throw new InvalidOperationException("Book not found.");
        }
        public bool TryGet(int componentId, out OrderItem orderItem)
        {
            var index = items.FindIndex(item => item.ComponentId == componentId);
            if (index == -1)
            {
                orderItem = null;
                return false;
            }
            orderItem = items[index];
            return true;
        }
        public OrderItem Add(int componentId, decimal price, int count)
        {
            if (TryGet(componentId, out OrderItem orderItem))
                throw new InvalidOperationException("Book already exists.");

            var orderItemDto = OrderItem.DtoFactory.Create(orderDto, componentId, price, count);
            orderDto.Items.Add(orderItemDto);

            orderItem = OrderItem.Mapper.Map(orderItemDto);
            items.Add(orderItem);

            return orderItem;
        }

        public void Remove(int componentId)
        {
            var index = items.FindIndex(item => item.ComponentId == componentId);
            if (index == -1)
                throw new InvalidOperationException("Can't find book to remove from order.");

            orderDto.Items.RemoveAt(index);
            items.RemoveAt(index);
        }
    }
}