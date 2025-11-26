using System;

namespace RfidWareHouseApp.Models
{
    public class InventoryItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string RfidTag { get; set; }
        public string Category { get; set; }
        public int Quantity { get; set; }
        public string Location { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
