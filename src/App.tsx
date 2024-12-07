import MainPage from "./pages/MainPage";
import { Route, BrowserRouter as Router, Routes } from "react-router-dom";
import { AnimatePresence } from "framer-motion";
import Navbar from "./components/main/NavBar";

function App() {
  return (
    <Router>
      <div className="min-h-screen bg-gradient-to-br from-blue-50 to-purple-100 text-gray-900">
        <Navbar/>
        
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
