import { motion } from "framer-motion";

export const AnimatedBackground = () => {
  return (
    <motion.div
      className="absolute inset-0 rounded-full"
      style={{
        background: "linear-gradient(45deg, #377dff, #5e72e4)",
        filter: "blur(40px)",
      }}
      animate={{
        scale: [1, 1.1, 1],
        rotate: [0, 10, 0],
      }}
      transition={{
        duration: 10,
        repeat: Infinity,
        ease: "easeInOut",
      }}
    />
  );
};
