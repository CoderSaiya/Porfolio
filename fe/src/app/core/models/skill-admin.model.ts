export interface SkillCategoryAdmin {
  id: string;
  title: string;
  icon: string;
  color: string;
  order: number;
  skills: SkillItemAdmin[];
}

export interface SkillItemAdmin {
  name: string;
  level: number;
  order: number;
}

export interface CreateSkillCategoryRequest {
  title: string;
  icon: string;
  color: string;
  order: number;
  skills: CreateSkillItemRequest[];
}

export interface CreateSkillItemRequest {
  name: string;
  level: number;
  order: number;
}

export interface UpdateSkillCategoryRequest {
  title?: string;
  icon?: string;
  color?: string;
  order?: number;
  skills?: UpdateSkillItemRequest[];
}

export interface UpdateSkillItemRequest {
  name?: string;
  level?: number;
  order?: number;
}

export interface ReorderCategoriesRequest {
  categories: CategoryOrder[];
}

export interface CategoryOrder {
  id: string;
  order: number;
}
