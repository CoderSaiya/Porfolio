namespace BE_Portfolio.DTOs;

public record ModifySkillsDto(
    List<string>? AddSkills,
    List<string>? RemoveSkills
    );