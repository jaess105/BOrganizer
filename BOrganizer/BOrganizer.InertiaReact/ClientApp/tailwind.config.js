/** @type {import('tailwindcss').Config} */
export default {
  content: [
    './index.html',
    './resources/**/*.{js,ts,jsx,tsx}',
    './src/**/*.{js,ts,jsx,tsx}',  // <-- include src if you're using it
  ],
  theme: {
    extend: {},
  },
  plugins: [],
}
