using Entities.Product;

namespace Entities.Kafka
{
    public class OrderMessage
    {
        public int UserId { get; set; }
        public int OrderId { get; set; }
        public List<productsIds> ProductIds { get; set; }
    }
}