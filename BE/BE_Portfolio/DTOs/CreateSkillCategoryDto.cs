namespace BE_Portfolio.DTOs;

public record CreateSkillCategoryDto(
    string Title,
    string IconName,
    string Color,
    List<string> Skills
    );