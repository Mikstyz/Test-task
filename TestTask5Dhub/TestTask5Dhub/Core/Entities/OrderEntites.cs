using Serilog;
using Entities.Product;


namespace Entities.Order
{
    public class Order // get \ create
    {
        public string Name { get; set; }
        public product[] gds{get; set;}
    }

    public class UpdateOrder
    {
        public int id { get; set; }

        public product[] NewGds { get; set; }
    }
}