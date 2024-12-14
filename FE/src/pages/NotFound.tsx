import React from 'react';
import { Link } from 'react-router-dom';
import { motion } from 'framer-motion';

const NotFound: React.FC = () => {
  return (
    <motion.div 
      initial={{ opacity: 0 }}
      animate={{ opacity: 1 }}
      className="min-h-screen flex items-center justify-center text-center"
    >
      <div>
        <h1 className="text-8xl font-bold text-transparent bg-clip-text bg-gradient-to-r from-blue-600 to-purple-600">
          404
        </h1>
        <p className="text-2xl mb-6">Trang bạn tìm kiếm không tồn tại</p>
        <Link 
          to="/" 
          className="bg-blue-600 text-white px-6 py-3 rounded-full hover:bg-blue-700 transition-colors"
        >
          Quay về Trang Chủ
        </Link>
      </div>
    </motion.div>
  );
};

export default NotFound;