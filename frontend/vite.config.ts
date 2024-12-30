import { defineConfig } from "vite";
import path from "path";
import react from "@vitejs/plugin-react-swc";

// https://vitejs.dev/config/
export default defineConfig({
  envDir: "./env",
  resolve: {
    alias: {
      "~": path.resolve("./src"),
    },
  },
  plugins: [react()],
  server: {
    hmr: { overlay: true },
    proxy: {
      "/api": "http://127.0.0.1:7827",
      "/uploads": "http://127.0.0.1:7827",
    },
  },
});
