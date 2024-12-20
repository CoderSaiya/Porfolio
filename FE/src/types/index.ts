export interface ProjectType {
    id: number,
    title: string,
    platform: string,
    position: string,
    numOfMember: number,
    description: string,
    imageUrl: string,
}

export interface ProjectWithTagType extends ProjectType {
    tags: string[]
}

export interface SkillType {
    id: number,
    nameSkill: string,
    level: number,
}

export interface ResponseType<T> {
    data: T,
    status: string;
}