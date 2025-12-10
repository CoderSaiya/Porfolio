namespace BE_Portfolio.DTOs.Skill;

public record UpdateSkillCategoryRequestDto
{
    public string? Title { get; init; }
    public string? Icon { get; init; }
    public string? Color { get; init; }
    public int? Order { get; init; }
    public List<UpdateSkillItemDTO>? Skills { get; init; }
}

public record UpdateSkillItemDTO
{
    public string? Name { get; init; }
    public int? Level { get; init; }
    public int? Order { get; init; }
}

public record ReorderCategoriesDTO
{
    public List<CategoryOrderDTO> Categories { get; init; } = new();
}

public record CategoryOrderDTO
{
    public string Id { get; init; } = null!;
    public int Order { get; init; }
}
