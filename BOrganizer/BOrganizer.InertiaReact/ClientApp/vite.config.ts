import {defineConfig} from "vite";
import react from "@vitejs/plugin-react";
import laravel from "laravel-vite-plugin";

const outDir = "../wwwroot/build";


export default defineConfig({
    plugins: [
        laravel({
            input: ["src/App.jsx"],
            publicDirectory: outDir,
            refresh: true,
        }),
        react()
    ],
    build: {
        emptyOutDir: true,
    },
})
