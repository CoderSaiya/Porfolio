import { motion } from "framer-motion";
import { useInView } from "react-intersection-observer";

const skills = [
  { name: ".NET", level: 90 },
  { name: "C#", level: 95 },
  { name: "React", level: 85 },
  { name: "JavaScript", level: 90 },
  { name: "TypeScript", level: 80 },
  { name: "SQL", level: 85 },
  { name: "React Native", level: 75 },
  { name: "Azure", level: 70 },
];

const SkillsSection = () => {
  const [ref, inView] = useInView({
    triggerOnce: true,
    threshold: 0.1,
  });

  return (
    <section id="skills" className="py-20 bg-gray-800">
      <div className="container mx-auto px-4">
        <motion.div
          ref={ref}
          initial={{ opacity: 0, y: 20 }}
          animate={inView ? { opacity: 1, y: 0 } : {}}
          transition={{ duration: 0.8 }}
          className="text-center mb-16"
        >
          <h2 className="text-4xl font-bold text-white mb-4">My Skills</h2>
          <p className="text-xl text-gray-400">
            Here are my technical skills and proficiency levels
          </p>
        </motion.div>
        <div className="grid md:grid-cols-2 gap-8">
          {skills.map((skill, index) => (
            <motion.div
              key={skill.name}
              initial={{ opacity: 0, x: -20 }}
              animate={inView ? { opacity: 1, x: 0 } : {}}
              transition={{ duration: 0.8, delay: index * 0.1 }}
            >
              <div className="flex justify-between mb-1">
                <span className="text-base font-medium text-white">
                  {skill.name}
                </span>
                <span className="text-sm font-medium text-white">
                  {skill.level}%
                </span>
              </div>
              <div className="w-full bg-gray-700 rounded-full h-2.5">
                <motion.div
                  className="bg-blue-500 h-2.5 rounded-full"
                  initial={{ width: 0 }}
                  animate={inView ? { width: `${skill.level}%` } : {}}
                  transition={{ duration: 1, delay: index * 0.1 }}
                ></motion.div>
              </div>
            </motion.div>
          ))}
        </div>
      </div>
    </section>
  );
};

export default SkillsSection;
