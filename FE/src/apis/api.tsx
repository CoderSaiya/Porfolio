import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { ProjectWithTagType, SkillType, ResponseType } from "../types";

const BASE_URL = import.meta.env.VITE_BASE_URL;

const baseQuery = fetchBaseQuery({
  baseUrl: `${BASE_URL}/api`,
  credentials: "include",
});

export const api = createApi({
  reducerPath: "api",
  baseQuery: baseQuery,
  endpoints: (builder) => ({
    getProjectWithTag: builder.query<ResponseType<ProjectWithTagType[]>, void>({
      query: () => "Project",
    }),
    getSkills: builder.query<ResponseType<SkillType[]>, void>({
      query: () => ({
        url: "Skill",
        method: "GET",
      }),
    }),
  }),
});

export const { useGetProjectWithTagQuery, useGetSkillsQuery } = api;
