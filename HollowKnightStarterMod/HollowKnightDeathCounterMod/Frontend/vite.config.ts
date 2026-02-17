import { defineConfig } from 'vite'
import { svelte } from '@sveltejs/vite-plugin-svelte'
import { viteSingleFile } from "vite-plugin-singlefile"


// https://vite.dev/config/
export default defineConfig({
  plugins: [svelte(), viteSingleFile(),],
  build: {
    target: 'esnext',  // optional but ensures modern JS
    minify: true,      // optional, reduces file size
    assetsInlineLimit: 100000000, // large limit to inline images/fonts
  },
});


