using MongoDB.Bson;

namespace BE_Portfolio.DTOs
{
    public class ProjectsWithTag
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Platform { get; set; }
        public string Position { get; set; }
        public int NumOfMember { get; set; }
        public string? Description { get; set; }
        public byte[] ImageData { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
    }
}
