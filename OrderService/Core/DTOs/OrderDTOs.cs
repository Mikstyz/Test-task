namespace DTOs.Order
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public List<int> ProductIds { get; set; }
    }

    public class CreateOrderDto
    {
        public int UserId { get; set; }
        public int[] ProductIds { get; set; }
    }

    public class GetOrdersByUserIdDto
    {
        public int UserId { get; set; }
        public int Offset { get; set; } = 0;
        public int Limit { get; set; } = 20;
    }
}