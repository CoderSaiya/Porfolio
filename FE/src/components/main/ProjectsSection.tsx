import { motion } from "framer-motion";
import { useInView } from "react-intersection-observer";

const projects = [
  {
    title: "E-commerce Platform",
    description: "A full-stack e-commerce solution using .NET Core and React.",
    image: "/placeholder.svg?height=300&width=400",
    tags: [".NET", "React", "SQL Server", "Azure"],
  },
  {
    title: "Mobile Fitness App",
    description:
      "Cross-platform mobile app for fitness tracking using React Native.",
    image: "/placeholder.svg?height=300&width=400",
    tags: ["React Native", "Firebase", "Redux"],
  },
  {
    title: "Task Management System",
    description: "Enterprise task management system with real-time updates.",
    image: "./asassets/react.svg",
    tags: [".NET", "SignalR", "React", "SQL Server"],
  },
  {
    title: "AI-powered Chatbot",
    description:
      "Intelligent chatbot for customer support using .NET and Azure AI.",
    image: "/placeholder.svg?height=300&width=400",
    tags: [".NET", "Azure AI", "React", "TypeScript"],
  },
];

export function ProjectsSection() {
  const [ref, inView] = useInView({
    triggerOnce: true,
    threshold: 0.1,
  });

  return (
    <section id="projects" className="py-20 bg-gray-900">
      <div className="container mx-auto px-4">
        <motion.div
          ref={ref}
          initial={{ opacity: 0, y: 20 }}
          animate={inView ? { opacity: 1, y: 0 } : {}}
          transition={{ duration: 0.8 }}
          className="text-center mb-16"
        >
          <h2 className="text-4xl font-bold text-white mb-4">My Projects</h2>
          <p className="text-xl text-gray-400">
            Here are some of my recent projects
          </p>
        </motion.div>
        <div className="grid md:grid-cols-2 gap-8">
          {projects.map((project, index) => (
            <motion.div
              key={project.title}
              initial={{ opacity: 0, y: 20 }}
              animate={inView ? { opacity: 1, y: 0 } : {}}
              transition={{ duration: 0.8, delay: index * 0.1 }}
              className="bg-gray-800 rounded-lg overflow-hidden shadow-lg"
            >
              <img
                src={project.image}
                alt={project.title}
                className="w-full h-48 object-cover"
              />
              <div className="p-6">
                <h3 className="text-2xl font-bold text-white mb-2">
                  {project.title}
                </h3>
                <p className="text-gray-400 mb-4">{project.description}</p>
                <div className="flex flex-wrap gap-2">
                  {project.tags.map((tag) => (
                    <span
                      key={tag}
                      className="px-3 py-1 bg-blue-500 text-white text-sm rounded-full"
                    >
                      {tag}
                    </span>
                  ))}
                </div>
              </div>
            </motion.div>
          ))}
        </div>
      </div>
    </section>
  );
}

export default ProjectsSection;
