import * as path from 'path';
import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

export default defineConfig({
    plugins: [react()],
    resolve: {
        alias: {
            "@": path.resolve(__dirname, "./src"),
        },
    },
    server: {
        port: 5173,
        proxy: {
            '/api': {
                target: 'https://localhost:8082',
                changeOrigin: true,
                secure: false,
            },
            '/hubs': {
                target: 'http://localhost:8081',  // HTTP for SignalR
                ws: true,
                changeOrigin: true,
            },
        },
    },
    base: '/',
})