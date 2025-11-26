using System;

namespace RfidWareHouseApp.Models
{
    public enum RepairStatus
    {
        New,
        InProgress,
        WaitingForParts,
        Completed,
        Closed
    }

    public class RepairTicket
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string DeviceName { get; set; }
        public string RfidTag { get; set; }
        public string IssueDescription { get; set; }
        public RepairStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string AssignedTechnician { get; set; }
    }
}
