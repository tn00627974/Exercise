using System.Collections.Generic;
using System.Threading.Tasks;

namespace RfidWareHouseApp.Services
{
    public interface IDataService
    {
        // Inventory
        Task<IEnumerable<Models.InventoryItem>> GetInventoryItemsAsync();
        Task<Models.InventoryItem> GetInventoryItemByRfidAsync(string rfidTag);
        Task UpdateInventoryItemAsync(Models.InventoryItem item);

        // Repair
        Task<IEnumerable<Models.RepairTicket>> GetRepairTicketsAsync();
        Task<Models.RepairTicket> GetRepairTicketByIdAsync(string id);
        Task CreateRepairTicketAsync(Models.RepairTicket ticket);
        Task UpdateRepairTicketAsync(Models.RepairTicket ticket);
    }
}
