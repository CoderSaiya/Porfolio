import { motion } from 'framer-motion'
import { useState } from 'react'

interface AnimatedButtonProps {
  children: React.ReactNode
  onClick?: () => void
}

export function AnimatedButton({ children, onClick }: AnimatedButtonProps) {
  const [isHovered, setIsHovered] = useState(false)

  return (
    <motion.button
      className="relative px-8 py-3 bg-transparent text-blue-500 rounded font-medium 
                 hover:text-white hover:bg-blue-500/10 transition-colors duration-300
                 uppercase tracking-wider text-sm border border-blue-500"
      onHoverStart={() => setIsHovered(true)}
      onHoverEnd={() => setIsHovered(false)}
      onClick={onClick}
      whileHover={{ scale: 1.02 }}
      whileTap={{ scale: 0.98 }}
    >
      <motion.div
        className="absolute inset-0 bg-transparent border border-blue-500/50 rounded"
        animate={{
          scale: isHovered ? [1, 1.05] : 1,
        }}
        transition={{
          duration: 1,
          repeat: Infinity,
          repeatType: "reverse"
        }}
      />
      <motion.div
        className="absolute inset-0 bg-transparent border border-blue-400/30 rounded"
        animate={{
          scale: isHovered ? [1.05, 1] : 1,
        }}
        transition={{
          duration: 1,
          repeat: Infinity,
          repeatType: "reverse"
        }}
      />
      {children}
    </motion.button>
  )
}