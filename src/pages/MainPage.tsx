import React from "react";
import Banner from "../components/main/Banner";
import SkillsSection from "../components/main/SkillsSection";
import ProjectsSection from "../components/main/ProjectsSection";

const MainPage: React.FC = () => {
  return (
    <div className="mt-10">
      <Banner />
      <SkillsSection />
      <ProjectsSection />
    </div>
  );
};

export default MainPage;
