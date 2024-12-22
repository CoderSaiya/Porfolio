import MainPage from "./pages/MainPage";
import { Route, BrowserRouter as Router, Routes } from "react-router-dom";
import { AnimatePresence } from "framer-motion";
import Navbar from "./components/main/Navbar";
import MouseTrail from "./components/main/MouseTrail";

function App() {
  return (
    <Router>
      <div className="min-h-screen bg-gradient-to-br from-blue-50 to-purple-100 text-gray-900">
        <MouseTrail />
        <Navbar />

        <AnimatePresence mode="wait">
          <Routes>
            <Route path="/" element={<MainPage />} />
          </Routes>
        </AnimatePresence>
      </div>
    </Router>
  );
}

export default App;
