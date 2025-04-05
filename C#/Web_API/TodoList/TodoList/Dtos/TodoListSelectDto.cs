namespace TodoList.Dtos
{
    public class TodoListSelectDto
    {
        public bool Enable { get; set; }
        public string? InsertEmployeeName { get; set; }
        public DateTime InsertTime { get; set; }
        public string? Name { get; set; }
        public int Orders { get; set; }
        public int TodoId { get; set; }
        public string? UpdateEmployeeName { get; set; }
        public DateTime? UpdateTime { get; set; } // 根據資料庫欄位是否允許 null
    }
}
