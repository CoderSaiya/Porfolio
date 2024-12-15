import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { ProjectWithTagType, SkillType } from "../types";

const BASE_URL = import.meta.env.VITE_BASE_URL;

const baseQuery = fetchBaseQuery({
  baseUrl: `${BASE_URL}/api`,
  credentials: "include",
});

export const api = createApi({
  reducerPath: "api",
  baseQuery: baseQuery,
  endpoints: (builder) => ({
    getProjectWithTag: builder.query<ProjectWithTagType[], void>({
      query: () => ({
        url: "Project",
        method: "POST",
      }),
    }),
    getSkills: builder.query<SkillType[], void>({
      query: () => ({
        url: "Skill",
        method: "POST",
      }),
    }),
  }),
});

export const { useGetProjectWithTagQuery, useGetSkillsQuery } = api;