namespace HelloWorld.Dtos
{
    public class NewsDto
    {
        public Guid NewsId { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public int Click { get; set; }
    }
}
