namespace Store.Web.App
{
    public class OrderItemModel
    {
        public int ComponentId { get; set; }

        public string NameOfComponent { get; set; }

        public string Package { get; set; }

        public int Count { get; set; }

        public decimal Price { get; set; }
    }
}
