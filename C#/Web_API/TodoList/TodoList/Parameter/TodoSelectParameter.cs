namespace TodoList.Parameter
{
    public class TodoSelectParameter
    {
        public string? name { get; set; }
        public bool? enable { get; set; }
        public DateTime? InsertTime { get; set; }
        public int? minOrder { get; set; }
        public int? maxOrder { get; set; }
    }
}
