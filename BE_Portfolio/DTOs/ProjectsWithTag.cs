namespace BE_Portfolio.DTOs
{
    public class ProjectsWithTag
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Platform { get; set; }
        public string Position { get; set; }
        public int NumOfMember { get; set; }
        public string? Description { get; set; }
        public string ImageUrl { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
    }
}
