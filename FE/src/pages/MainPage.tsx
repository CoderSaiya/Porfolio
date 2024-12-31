import React, { useEffect } from "react";
import Banner from "../components/main/Banner";
import SkillsSection from "../components/main/SkillsSection";
import ProjectsSection from "../components/main/ProjectsSection";
import AboutSection from "../components/main/AboutSection";
import ContractSection from "../components/main/ContractSection";
import AOS from "aos";
import "aos/dist/aos.css";

const MainPage: React.FC = () => {
  useEffect(() => {
    AOS.init({
      duration: 1000,
      once: false,
      mirror: false,
    });
  }, []);

  return (
    <div className="mt-10">
      <Banner />
      <AboutSection />
      <SkillsSection />
      <ProjectsSection />
      <ContractSection />
    </div>
  );
};

export default MainPage;
