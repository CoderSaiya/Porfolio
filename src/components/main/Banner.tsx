import { motion } from "framer-motion";
import { useEffect, useState } from "react";
import { AnimatedText } from "./AnimatedText";
import { ThreeScene } from "./ThreeScene";
import { AnimatedBackground } from "./AnimatedBackground";
import { TypingAnimation } from "./TypingAnimation";
import { AnimatedButton } from "./AnimatedButton";

const colors = [
  "#FFF",
  "#FFD700",
  "#FF69B4",
  "#00CED1",
  "#FF6347",
  "#7FFFD4",
  "#DDA0DD",
];

const ShootingStar = () => {
  const randomStart = {
    x: Math.random() * 100,
    y: Math.random() * 50,
  };

  const size = Math.random() * 2 + 1; // Random size between 1 and 3 pixels
  const color = colors[Math.floor(Math.random() * colors.length)];

  return (
    <motion.div
      className="absolute rounded-full"
      style={{
        width: size,
        height: size,
        backgroundColor: color,
      }}
      initial={{
        top: `${randomStart.y}%`,
        left: `${randomStart.x}%`,
        opacity: 0,
      }}
      animate={{
        top: ["0%", "100%"],
        left: [`${randomStart.x}%`, `${randomStart.x + 20}%`],
        opacity: [0, 1, 1, 0],
      }}
      transition={{
        duration: 2,
        repeat: Infinity,
        repeatDelay: Math.random() * 5 + 3,
      }}
    />
  );
};

const Banner = () => {
  const [stars, setStars] = useState<number[]>([]);

  useEffect(() => {
    setStars(Array.from({ length: 20 }, (_, i) => i));
  }, []);

  return (
    <section
      id="home"
      className="min-h-screen bg-gray-900 relative overflow-hidden"
    >
      <div className="absolute inset-0">
        <ThreeScene />
      </div>
      {stars.map((_, index) => (
        <ShootingStar key={index} />
      ))}
      <div className="relative container mx-auto px-4 pt-32 flex flex-col md:flex-row items-center justify-between">
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.8 }}
          className="md:w-1/2 text-center md:text-left mb-8 md:mb-0"
        >
          <h1 className="text-5xl md:text-7xl font-bold text-white mb-6">
            Nhật Cường
            <br />
            <span className="text-blue-500">
              Backend <br /> <TypingAnimation words={["Developer"]} />
            </span>
          </h1>
          <AnimatedText
            words={[
              ".NET Expert",
              "React Enthusiast",
              "Mobile App Developer",
              "Web Wizard",
            ]}
            className="text-xl md:text-2xl text-pink-500 mb-8"
          />
          <AnimatedButton>See More About Us</AnimatedButton>
        </motion.div>
        <motion.div
          initial={{ opacity: 0, scale: 0.5 }}
          animate={{ opacity: 1, scale: 1 }}
          transition={{ duration: 0.8 }}
          className="md:w-1/3 flex justify-center"
        >
          <div className="relative w-70 h-70 md:w-80 md:h-80">
            <AnimatedBackground />
            <motion.img
              src="imgs/avatar.png"
              alt="Nhật Cường"
              className="absolute inset-2 rounded-full object-cover"
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              transition={{ duration: 1, delay: 0.5 }}
            />
          </div>
        </motion.div>
      </div>
    </section>
  );
};

export default Banner;
