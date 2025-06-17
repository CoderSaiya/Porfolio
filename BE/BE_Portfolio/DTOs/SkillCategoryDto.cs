namespace BE_Portfolio.DTOs;

public record SkillCategoryDto(
    Guid Id,
    string Title,
    string Icon,
    string Color,
    List<string> Skills
    );