﻿namespace TP4SCS.Library.Models.Request.Service
{
    public class ServiceRequest
    {
        public int BranchId { get; set; }

        public int CategoryId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal Price { get; set; }
    }
}
