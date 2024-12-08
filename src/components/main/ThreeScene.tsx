import { Canvas, useFrame } from '@react-three/fiber'
import { useRef } from 'react'
import { Mesh } from 'three'
import { Environment, Float, PerspectiveCamera } from '@react-three/drei'

function AnimatedSphere() {
  const meshRef = useRef<Mesh>(null)

  useFrame((state) => {
    if (meshRef.current) {
      meshRef.current.rotation.y = state.clock.getElapsedTime() * 0.5
      meshRef.current.position.y = Math.sin(state.clock.getElapsedTime()) * 0.5
    }
  })

  return (
    <Float speed={1.4} rotationIntensity={1} floatIntensity={2}>
      <mesh ref={meshRef}>
        <torusKnotGeometry args={[1, 0.3, 128, 16]} />
        <meshStandardMaterial
          color="#00a3ff"
          roughness={0.2}
          metalness={0.8}
        />
      </mesh>
    </Float>
  )
}

export function ThreeScene() {
  return (
    <div className="h-[50vh] w-full">
      <Canvas>
        <PerspectiveCamera makeDefault position={[0, 0, 5]} />
        <AnimatedSphere />
        <Environment preset="night" />
      </Canvas>
    </div>
  )
}