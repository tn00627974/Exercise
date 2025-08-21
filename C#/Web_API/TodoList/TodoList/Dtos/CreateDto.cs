namespace Todo.Dtos
{
    public class CreateDto
    {
        public string? Name { get; set; }
        public DateTime InsertTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public bool Enable { get; set; }
        public int Orders { get; set; }
        public string? InsertEmployeeName { get; set; }
        public string? UpdateEmployeeName { get; set; }
    }
}
