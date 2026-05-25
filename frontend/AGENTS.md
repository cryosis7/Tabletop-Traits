# Frontend - Board Game Rankings

## Overview

React 19 single-page application (TypeScript, Vite) that displays board game collection data and mechanism analysis charts. Communicates with the .NET backend API via Axios.

## Tech Stack

- React 19 with functional components and hooks
- TypeScript (strict mode)
- Vite for build tooling and dev server
- Recharts for data visualization
- Axios for HTTP requests
- ESLint for linting

## Project Structure

| Directory | Responsibility |
|-----------|----------------|
| `src/types/` | TypeScript interfaces and type definitions shared across the app. |
| `src/services/` | Axios-based API client functions. One function per endpoint. |
| `src/hooks/` | Custom React hooks (data fetching, state management). |
| `src/components/charts/` | Recharts chart components for data visualization. |
| `src/pages/` | Page-level components composed of smaller components. |
| `src/assets/` | Static assets (images, icons). |

### Key Conventions

- **Types first.** Define interfaces in `src/types/` before using them in services or components.
- **API layer is isolated.** Components never call Axios directly; they use hooks which call service functions.
- **Custom hooks** encapsulate data fetching, loading states, and error states.
- **Chart components** are self-contained; they receive typed data via props and render Recharts elements.

## Coding Standards

### Naming

- PascalCase for components, types, and interfaces.
- camelCase for variables, functions, hooks, and file names (except components).
- Component files use PascalCase (`Dashboard.tsx`).
- Hook files are prefixed with `use` (`useApi.ts`).
- Prefix interfaces with descriptive nouns, not `I` (e.g., `BoardGame`, not `IBoardGame`).

### Patterns to Follow

- Functional components only; no class components.
- Destructure props in the function signature.
- Use explicit return types on exported functions and hooks.
- Co-locate related logic in custom hooks rather than spreading `useState`/`useEffect` across components.
- Keep components focused: one responsibility per component.
- Use TypeScript `interface` for object shapes, `type` for unions/intersections.

### Anti-Patterns to Avoid

- Do not use `any` type. Use `unknown` if the type is genuinely unknown, then narrow.
- Do not call API services directly from components; wrap in a custom hook.
- Do not mutate state directly; always use setter functions or produce new references.
- Do not suppress TypeScript or ESLint errors without a comment explaining why.
- Do not inline large data transformations in JSX; extract to a named function or useMemo.
- Do not use `useEffect` for derived state; compute it during render or use `useMemo`.

## Testing

When tests are added, follow these conventions:

- Prefer React Testing Library for component tests.
- Test user-visible behavior, not implementation details.
- Mock API calls at the service layer (not Axios internals).
- Use descriptive test names: `renders loading state while fetching`, `displays error message on failure`.

## Error Handling

- API hooks should expose `{ data, loading, error }` states.
- Display user-friendly error messages in the UI; log details to the console.
- Handle loading states with appropriate skeletons or spinners.
- Validate API response shapes with TypeScript (compile-time) and optional runtime checks for critical data.

## Key Commands

```powershell
# Install dependencies
npm install

# Start dev server (hot reload)
npm run dev

# Type check
npx tsc --noEmit

# Lint
npm run lint

# Production build
npm run build
```
