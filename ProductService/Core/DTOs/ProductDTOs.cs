using Entities.Product;

namespace DTOs.Product
{
    public class CheckAvailabilityDto
    {
        public int productId { get; set; }
        public int Quantity { get; set; }
    }

    public class CreateOrderDto
    {
        public List<productsIds> ProductsIds { get;set; }
    }

    public class ProductDto
    {
        public int Seller { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }

    public class CreateProductDto
    {
        public int Seller { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateProductDto
    {
        public int Id { get; set; }
        public string NewName { get; set; }
        public string NewDescription { get; set; }
        public double NewPrice { get; set; }
        public int NewQuantity { get; set; }
    }

    public class SearchProductDto
    {
        public int Offset { get; set; } = 0;
        public string? Seller { get; set; }
        public string? Name { get; set; }
        public double? Price { get; set; }
        public int? Quantity { get; set; }
    }
}