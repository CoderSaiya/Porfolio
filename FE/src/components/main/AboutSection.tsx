import { motion } from "framer-motion";
import { TypingAnimation } from "./TypingAnimation";
import AOS from "aos";
import "aos/dist/aos.css";
import { useEffect } from "react";

const AboutSection = () => {
  useEffect(() => {
    const initAOS = () => {
      AOS.init({
        once: true,
        offset: 10,
      });
    };

    initAOS();
    window.addEventListener("resize", initAOS);
    return () => window.removeEventListener("resize", initAOS);
  }, []);

  return (
    <section id="about" className="py-20 bg-gray-800">
      <div className="container mx-auto px-4">
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.8 }}
          className="text-center mb-12"
        >
          <h2 className="text-4xl font-bold text-white mb-4">About Me</h2>
          <div className="text-2xl text-blue-500">
            <TypingAnimation
              words={[
                "Passionate Developer",
                "Continuous Learner",
                "Problem Solver",
                "Team Player",
              ]}
            />
          </div>
        </motion.div>
        <div className="grid md:grid-cols-2 gap-8">
          <motion.div
            initial={{ opacity: 0, x: -20 }}
            animate={{ opacity: 1, x: 0 }}
            transition={{ duration: 0.8, delay: 0.2 }}
          >
            <h3 className="text-2xl font-semibold text-white mb-4">
              Education
            </h3>
            <ul className="space-y-4" data-aos="fade-right">
              <li>
                <h4 className="text-xl text-blue-400">
                  Information Technology
                </h4>
                <p className="text-gray-300">
                  University of Transport Ho Chi Minh City, 2022-now
                </p>
              </li>
            </ul>
          </motion.div>
          <motion.div
            initial={{ opacity: 0, x: 20 }}
            animate={{ opacity: 1, x: 0 }}
            transition={{ duration: 0.8, delay: 0.4 }}
          >
            <h3 className="text-2xl font-semibold text-white mb-4">
              Professional Info
            </h3>
            <ul className="space-y-2 text-gray-300" data-aos="fade-left">
              <li>• Expertise in .NET, React, and mobile app development</li>
              <li>• Strong problem-solving and analytical skills</li>
              <li>• Excellent communicator and team collaborator</li>
              <li>• Passionate about clean code and best practices</li>
            </ul>
          </motion.div>
        </div>
      </div>
    </section>
  );
};

export default AboutSection;
