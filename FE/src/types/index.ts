export interface ProjectType {
    Id: number,
    Title: string,
    Platform: string,
    Position: string,
    NumOfMember: number,
    Description: string,
    ImageUrl: string,
}

export interface ProjectWithTagType extends ProjectType {
    Tags: string[]
}

export interface SkillType {
    Id: number,
    NameSkill: string,
    Level: number,
}