
namespace Entities.Product
{
    public class product //get \ create
    {
        public string Seller { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }


    public class updateProduct
    {
        public int id { get; set; }

        public string NewName { get; set; }
        public string NewDescription { get; set; }
        public double NewPrice { get; set; }
        public int NewQuantity { get; set; }
    }
}