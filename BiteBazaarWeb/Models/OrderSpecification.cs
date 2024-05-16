﻿using System.ComponentModel.DataAnnotations.Schema;

namespace BiteBazaarWeb.Models
{
    public class OrderSpecification
    {
        public int OrderSpecificationId { get; set; }

        [ForeignKey("Order")]
        public int FkOrderId { get; set; }
        public Order? Order { get; set; }

        public int Count { get; set; }
        [ForeignKey("Product")]
        public int FkProductId { get; set; }
        public Product? Product { get; set; }
    }
}