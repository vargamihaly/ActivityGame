/** @type {import('tailwindcss').Config} */
export default {
    darkMode: ["class"],
    content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
  	extend: {
  		borderRadius: {
  			lg: 'var(--radius)',
  			md: 'calc(var(--radius) - 2px)',
  			sm: 'calc(var(--radius) - 4px)'
  		},
        typography: {
            DEFAULT: {
                css: {
                    h1: {
                        color: '#1f2937',
                        fontWeight: '700',
                    },
                    p: {
                        color: '#4b5563',
                    },
                },
            },
        },
  		colors: {}
  	}
  },
  plugins: [require("tailwindcss-animate"), require("@tailwindcss/typography")],
}