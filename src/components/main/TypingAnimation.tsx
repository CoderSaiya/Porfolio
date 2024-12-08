import { useState, useEffect } from 'react'
import { motion } from 'framer-motion'

interface TypingAnimationProps {
  words: string[]
  typingSpeed?: number
  deletingSpeed?: number
  delayBetweenWords?: number
}

export function TypingAnimation({ 
  words, 
  typingSpeed = 150, 
  deletingSpeed = 100, 
  delayBetweenWords = 1000 
}: TypingAnimationProps) {
  const [currentWordIndex, setCurrentWordIndex] = useState(0)
  const [currentText, setCurrentText] = useState('')
  const [isDeleting, setIsDeleting] = useState(false)

  useEffect(() => {
    let timeout: NodeJS.Timeout

    const animateText = () => {
      const currentWord = words[currentWordIndex]
      
      if (!isDeleting) {
        setCurrentText(currentWord.substring(0, currentText.length + 1))
        
        if (currentText === currentWord) {
          timeout = setTimeout(() => setIsDeleting(true), delayBetweenWords)
          return
        }
      } else {
        setCurrentText(currentWord.substring(0, currentText.length - 1))
        
        if (currentText === '') {
          setIsDeleting(false)
          setCurrentWordIndex((prevIndex) => (prevIndex + 1) % words.length)
          return
        }
      }

      timeout = setTimeout(animateText, isDeleting ? deletingSpeed : typingSpeed)
    }

    timeout = setTimeout(animateText, typingSpeed)

    return () => clearTimeout(timeout)
  }, [currentText, isDeleting, currentWordIndex, words, typingSpeed, deletingSpeed, delayBetweenWords])

  return (
    <div className="inline-block">
      {currentText}
      <motion.span
        animate={{ opacity: [1, 0] }}
        transition={{ duration: 0.5, repeat: Infinity, repeatType: 'reverse' }}
        className="inline-block w-[2px] h-[1em] bg-blue-500 ml-1 align-middle"
      />
    </div>
  )
}