import { motion } from "framer-motion";
import { AnimatedText } from "./AnimatedText";
import { ThreeScene } from "./ThreeScene";

const Banner = () => {
  return (
    <section id="home" className="min-h-screen bg-gray-900 relative overflow-hidden">
      <div className="absolute inset-0">
        <ThreeScene />
      </div>
      <div className="relative container mx-auto px-4 pt-32">
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.8 }}
          className="max-w-4xl mx-auto text-center"
        >
          <h1 className="text-5xl md:text-7xl font-bold text-white mb-6">
            Nhật Cường
            <br />
            <span className="text-blue-500">Backend Developer</span>
          </h1>
          <AnimatedText
            words={[
              ".NET Expert",
              "React Enthusiast",
              "Mobile App Developer",
              "Web Wizard"
            ]}
            className="text-xl md:text-2xl text-pink-500 mb-8"
          />
          <motion.button
            whileHover={{ scale: 1.05 }}
            whileTap={{ scale: 0.95 }}
            className="px-8 py-3 bg-blue-500 text-white rounded-full hover:bg-blue-600 transition-colors"
          >
            View My Work
          </motion.button>
        </motion.div>
      </div>
    </section>
  );
};

export default Banner;
