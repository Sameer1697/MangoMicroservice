﻿using System.ComponentModel.DataAnnotations;

namespace Mango.Services.EmailAPI.Models.DTO
{
    public class ProductDTO
    {
       
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string Categoryname { get; set; }
        public string ImageUrl { get; set; }

        [Range(1,100)]
        public int Count { get; set; } = 1;
    }
}
