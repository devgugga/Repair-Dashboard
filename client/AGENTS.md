# Repository Guidelines

## Project Structure & Module Organization
- Application code lives in `src/` with feature-oriented folders: `src/app` (app shell/providers/styles), `src/pages` (route-level views), `src/features` (user-facing features), and `src/shared` (reusable config/types/services).
- Tests are colocated as `*.spec.ts` files (example: `src/app/App.spec.ts`). Test setup is in `src/test/setup.ts`.
- Static assets are in `public/`. Build output is generated to `dist/`.
- Path aliases are configured in Vite/Vitest: `@`, `@app`, `@pages`, `@features`, `@shared`.

## Build, Test, and Development Commands
- `pnpm dev`: start Vite dev server.
- `pnpm build`: run type-check then production build.
- `pnpm preview`: preview the production bundle locally.
- `pnpm test`: run Vitest once in CI mode.
- `pnpm test:watch`: run tests in watch mode.
- `pnpm lint` / `pnpm lint:fix`: run or auto-fix `oxlint` + `eslint`.
- `pnpm format` / `pnpm format:check`: apply or verify formatting with `oxfmt`.
- `pnpm check`: local quality gate (`format:check`, `lint`, `typecheck`).
- `pnpm ci:check`: CI-style validation (`lint`, `typecheck`, `test`, `build-only`).

## Coding Style & Naming Conventions
- Use 2-space indentation, UTF-8, LF endings, trailing-whitespace trim, and ~100-char lines (see `.editorconfig`).
- Prefer TypeScript in Vue SFCs and keep imports typed consistently.
- Use PascalCase for Vue components (`DashboardPage.vue`), camelCase for composables/utilities (`useTheme.ts`), and `*.spec.ts` for tests.
- Run `pnpm format && pnpm lint` before opening a PR.

## Testing Guidelines
- Framework: Vitest + `@vue/test-utils` with `jsdom` environment.
- Test files must match `src/**/*.spec.ts`.
- Coverage uses V8 reporters (`text`, `html`); no hard threshold is enforced yet, so add meaningful assertions for new logic and UI states.

## Commit & Pull Request Guidelines
- Current history is small and mixed-language; adopt clear, imperative commits moving forward (e.g., `feat(theme): add persisted dark mode`).
- Keep commits focused and runnable.
- PRs should include: purpose summary, linked issue (if any), test notes (`pnpm test`, `pnpm check`), and screenshots/GIFs for UI changes.
- Husky runs `lint-staged` on commit; ensure staged files pass formatting/lint fixes before pushing.
