import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import laravel from "laravel-vite-plugin";
import path from 'path';
import tailwindcss from '@tailwindcss/vite'




const outDir = "../wwwroot/build";


export default defineConfig({
    plugins: [
        laravel({
            input: ["src/css/app.css", "src/js/main.tsx"],
            ssr: 'src/js/ssr.tsx',
            publicDirectory: outDir,
            refresh: true,
        }),
        react(),
        tailwindcss(),
    ],
    esbuild: {
        jsx: 'automatic'
    },
    build: {
        emptyOutDir: true,
    },
    resolve: {
        alias: {
            '@': path.resolve(__dirname, 'src/js'),
        },
    },
})
