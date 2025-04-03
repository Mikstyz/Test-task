﻿using Serilog;
using Entities.Product;


namespace Entities.Order
{
    public class Order // get \ create
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public List<int> ProductIds{get; set;}
    }

    public class UpdateOrder
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public List<int> NewProductIds { get; set; }
    }
}