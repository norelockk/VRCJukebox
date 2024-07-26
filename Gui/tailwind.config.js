const defaultColors = require('tailwindcss/colors')

/** @type {import('tailwindcss').Config} */
export default {
  // prefix: 'gui',
  // separator: '_',
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx,vue}",
  ],
  theme: {
    colors: {
      ...defaultColors,
      'accent': '#7e5bef',
      'primary': '#1fb6ff',
      'b-accent': '#333333',
      'b-primary': '#2b2b2b'
    },
    extend: {},
    fontFamily: {
      sans: ['Poppins', 'sans-serif'],
    },
  },
  plugins: [],
}

