namespace BE_Portfolio.DTOs;

public class CreateProjectDto
{
    public string Title { get; set; } = null!;
    public string Platform { get; set; } = null!;
    public string Position { get; set; } = null!;
    public int NumOfMember { get; set; }
    public string? Description { get; set; }
    public string? Link { get; set; }
    public string? Demo { get; set; }
    public IFormFile ImageFile { get; set; } = null!;
    public List<string> Tags { get; set; } = new();
}