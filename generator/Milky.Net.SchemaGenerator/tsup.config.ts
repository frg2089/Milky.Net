import { defineConfig } from 'tsup'

export default defineConfig({
  entry: ['src/gen_types.ts'],
  format: 'esm',
  dts: false,
  sourcemap: true,
  clean: true,
})
