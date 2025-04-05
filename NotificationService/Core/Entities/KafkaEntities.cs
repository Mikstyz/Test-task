using Entities.Product;

namespace Entities.Kafka
{
    public class OrderMessage
    {
        public string UserId { get; set; }
        public List<productsIds> ProductIds { get; set; }
    }
}

namespace Entities.Product
{
    public class productsIds
    {
        public int productId { get; set; }
        public int Quantity { get; set; }
    }
}