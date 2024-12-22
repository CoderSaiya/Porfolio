import { Point } from 'framer-motion';
import { useState, useEffect } from 'react';
import { MousePosition } from '../../types';

const MouseTrail: React.FC = () => {
  const [trails, setTrails] = useState<Point[]>([]);
  const [mousePosition, setMousePosition] = useState<MousePosition>({ x: 0, y: 0 });

  useEffect(() => {
    let animationFrameId: number;
    let trailPoints: Point[] = [];

    const handleMouseMove = (e: MouseEvent): void => {
      setMousePosition({ x: e.clientX, y: e.clientY });

      trailPoints.unshift({ 
        x: e.clientX, 
        y: e.clientY, 
        opacity: 1 
      });

      if (trailPoints.length > 20) {
        trailPoints = trailPoints.slice(0, 20);
      }

      trailPoints = trailPoints.map((point, index) => ({
        ...point,
        opacity: Math.max(1 - index / 20, 0)
      }));

      setTrails([...trailPoints]);
    };

    const animate = (): void => {
      setTrails(prevTrails => 
        prevTrails
          .map(trail => ({
            ...trail,
            opacity: Math.max(trail.opacity - 0.015, 0)
          }))
          .filter(trail => trail.opacity > 0)
      );

      animationFrameId = requestAnimationFrame(animate);
    };

    window.addEventListener('mousemove', handleMouseMove);
    animationFrameId = requestAnimationFrame(animate);

    return () => {
      window.removeEventListener('mousemove', handleMouseMove);
      cancelAnimationFrame(animationFrameId);
    };
  }, []);

  return (
    <div style={{
      position: 'fixed',
      inset: 0,
      pointerEvents: 'none',
      zIndex: 9999,
    }}>
      <svg style={{
        width: '100%',
        height: '100%',
        position: 'absolute',
        top: 0,
        left: 0,
      }}>
        {trails.length > 1 && (
          <path
            d={`M ${trails.map(point => `${point.x} ${point.y}`).join(' L ')}`}
            fill="none"
            stroke="rgba(0, 255, 255, 0.8)"
            strokeWidth="2"
            style={{ filter: 'blur(4px)' }}
          />
        )}
        
        {trails.map((trail, index) => (
          <circle
            key={index}
            cx={trail.x}
            cy={trail.y}
            r={index === 0 ? 4 : 2}
            fill={`rgba(0, 255, 255, ${trail.opacity})`}
            style={{ filter: 'blur(4px)' }}
          />
        ))}
      </svg>
    </div>
  );
};

export default MouseTrail;