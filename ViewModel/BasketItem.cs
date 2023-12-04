﻿namespace PesKitTask.ViewModel
{
    public class BasketItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Count { get; set; }
        public decimal SubTotal { get; set; }
    }
}