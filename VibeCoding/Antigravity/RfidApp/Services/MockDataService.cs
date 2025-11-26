using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RfidApp.Models;

namespace RfidApp.Services
{
    public class MockDataService : IDataService
    {
        private List<InventoryItem> _inventory;
        private List<RepairTicket> _repairTickets;

        public MockDataService()
        {
            InitializeMockData();
        }

        private void InitializeMockData()
        {
            _inventory = new List<InventoryItem>
            {
                new InventoryItem { Name = "Laptop Dell XPS", RfidTag = "RFID-001", Category = "Computers", Quantity = 10, Location = "A-1" },
                new InventoryItem { Name = "Monitor LG 27", RfidTag = "RFID-002", Category = "Monitors", Quantity = 5, Location = "B-2" },
                new InventoryItem { Name = "Keyboard Mech", RfidTag = "RFID-003", Category = "Accessories", Quantity = 20, Location = "C-1" }
            };

            _repairTickets = new List<RepairTicket>
            {
                new RepairTicket { DeviceName = "Projector Epson", RfidTag = "RFID-999", IssueDescription = "Bulb burnt out", Status = RepairStatus.New, CreatedAt = DateTime.Now.AddDays(-1) }
            };
        }

        public Task<IEnumerable<InventoryItem>> GetInventoryItemsAsync()
        {
            return Task.FromResult<IEnumerable<InventoryItem>>(_inventory);
        }

        public Task<InventoryItem> GetInventoryItemByRfidAsync(string rfidTag)
        {
            return Task.FromResult(_inventory.FirstOrDefault(i => i.RfidTag == rfidTag));
        }

        public Task UpdateInventoryItemAsync(InventoryItem item)
        {
            var existing = _inventory.FirstOrDefault(i => i.Id == item.Id);
            if (existing != null)
            {
                existing.Quantity = item.Quantity;
                existing.Location = item.Location;
                existing.LastUpdated = DateTime.Now;
            }
            return Task.CompletedTask;
        }

        public Task<IEnumerable<RepairTicket>> GetRepairTicketsAsync()
        {
            return Task.FromResult<IEnumerable<RepairTicket>>(_repairTickets);
        }

        public Task<RepairTicket> GetRepairTicketByIdAsync(string id)
        {
            return Task.FromResult(_repairTickets.FirstOrDefault(t => t.Id == id));
        }

        public Task CreateRepairTicketAsync(RepairTicket ticket)
        {
            _repairTickets.Add(ticket);
            return Task.CompletedTask;
        }

        public Task UpdateRepairTicketAsync(RepairTicket ticket)
        {
             var existing = _repairTickets.FirstOrDefault(t => t.Id == ticket.Id);
            if (existing != null)
            {
                existing.Status = ticket.Status;
                existing.CompletedAt = ticket.CompletedAt;
                existing.AssignedTechnician = ticket.AssignedTechnician;
            }
            return Task.CompletedTask;
        }
    }
}
