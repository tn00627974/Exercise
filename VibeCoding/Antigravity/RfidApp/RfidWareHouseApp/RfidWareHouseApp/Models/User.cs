namespace RfidApp.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Role { get; set; } // "Admin", "Warehouse", "Technician"
        public string FullName { get; set; }
    }
}
