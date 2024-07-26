import vue from '@vitejs/plugin-vue'

import { resolve } from 'path';
import { defineConfig, splitVendorChunkPlugin } from 'vite'

export default defineConfig({
  base: '/',
  build: {
    sourcemap: true
  },
  resolve: {
    alias: {
      '@': resolve(__dirname, './src')
    }
  },
  plugins: [
    vue(),
    splitVendorChunkPlugin()
  ],
});