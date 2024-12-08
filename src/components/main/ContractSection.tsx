import { useState } from "react";
import { motion } from "framer-motion";
import { Send } from "lucide-react";

const ContractSection = () => {
  const [formState, setFormState] = useState({
    name: "",
    email: "",
    message: "",
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    //xu ly su kien submit (se update sau)
    console.log("Form submitted:", formState);
  };

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ) => {
    setFormState((prev) => ({ ...prev, [e.target.name]: e.target.value }));
  };

  return (
    <section id="contact" className="py-20 bg-gray-900">
      <div className="container mx-auto px-4">
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.8 }}
          className="text-center mb-12"
        >
          <h2 className="text-4xl font-bold text-white mb-4">Get In Touch</h2>
          <p className="text-xl text-gray-400">
            Have a question or want to work together?
          </p>
        </motion.div>
        <motion.form
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.8, delay: 0.2 }}
          onSubmit={handleSubmit}
          className="max-w-lg mx-auto"
        >
          <div className="mb-4">
            <input
              type="text"
              name="name"
              placeholder="Your Name"
              value={formState.name}
              onChange={handleChange}
              className="w-full px-4 py-2 bg-gray-800 text-white rounded focus:outline-none focus:ring-2 focus:ring-blue-500"
              required
            />
          </div>
          <div className="mb-4">
            <input
              type="email"
              name="email"
              placeholder="Your Email"
              value={formState.email}
              onChange={handleChange}
              className="w-full px-4 py-2 bg-gray-800 text-white rounded focus:outline-none focus:ring-2 focus:ring-blue-500"
              required
            />
          </div>
          <div className="mb-4">
            <textarea
              name="message"
              placeholder="Your Message"
              value={formState.message}
              onChange={handleChange}
              rows={4}
              className="w-full px-4 py-2 bg-gray-800 text-white rounded focus:outline-none focus:ring-2 focus:ring-blue-500"
              required
            ></textarea>
          </div>
          <motion.button
            whileHover={{ scale: 1.05 }}
            whileTap={{ scale: 0.95 }}
            className="w-full px-6 py-3 bg-blue-500 text-white rounded hover:bg-blue-600 transition-colors flex items-center justify-center"
            type="submit"
          >
            Send Message
            <Send className="ml-2 h-5 w-5" />
          </motion.button>
        </motion.form>
      </div>
    </section>
  );
};

export default ContractSection;
