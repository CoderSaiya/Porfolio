namespace BE_Portfolio.DTOs.Skill;

public record CreateSkillCategoryRequestDto
{
    public string Title { get; init; } = null!;
    public string Icon { get; init; } = null!;
    public string Color { get; init; } = "#3b82f6";
    public int Order { get; init; }
    public List<CreateSkillItemDTO> Skills { get; init; } = new();
}

public record CreateSkillItemDTO
{
    public string Name { get; init; } = null!;
    public int Level { get; init; }
    public int Order { get; init; }
}
