import { motion } from "framer-motion";
import { Link } from "react-router-dom";
import { useScrollControl } from "../../hook/useScrollControl";

export function NavBar() {
  const { scrollTo } = useScrollControl();

  return (
    <motion.nav 
      initial={{ y: -100 }}
      animate={{ y: 0 }}
      className="fixed top-0 left-0 right-0 z-50 bg-gray-900/80 backdrop-blur-sm"
    >
      <div className="container mx-auto px-4">
        <div className="flex items-center justify-between h-16">
          <Link to="/" className="text-2xl font-bold text-white">
            Nhật Cường - CoderSaiya
          </Link>
          <div className="hidden md:flex space-x-8">
            {['home', 'about', 'skills', 'projects', 'contact'].map((item) => (
              <button
                key={item}
                onClick={() => scrollTo(item)}
                className="text-gray-300 hover:text-blue-500 transition-colors duration-300 uppercase tracking-wider text-sm"
              >
                {item}
              </button>
            ))}
          </div>
        </div>
      </div>
    </motion.nav>
  );
}

export default NavBar;
