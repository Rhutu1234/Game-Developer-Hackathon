Game-Developer-Hackathon — Quick Start

Overview

This repository is a full-stack sample: a .NET backend (Game-Developer-Hackathon) and an Angular frontend (UI).
- Backend: C# .NET 10 Web API (serves API and can host built frontend assets)
- Frontend: Angular (standalone AppComponent) in UI/

Final development URLs
- Backend API (when running): https://localhost:51979
  - API root: https://localhost:51979/api/matchintelligence
- Frontend (dev server): http://localhost:4200 (ng serve, proxies /api/* to backend)
- Frontend (production served by backend after build): https://localhost:51979/

Prerequisites
- .NET 10 SDK and dotnet on PATH
  - Verify: dotnet --info
- Node.js LTS on PATH (for Angular dev/build)
  - Verify: node -v && npm -v
- (Optional) Trust dev HTTPS certificate for backend:
  dotnet dev-certs https --trust

Backend (API)

Run (PowerShell)
1. From repo root:
   cd C:\Users\rhutu\source\repos\Game-Developer-Hackathon
2. Run API:
   dotnet run --project Game-Developer-Hackathon.csproj
3. Verify (direct call):
   curl -k "https://localhost:51979/api/matchintelligence/generate-events?count=5"

Frontend (Angular)

Install dependencies (once)
1. Open a new terminal and run:
   cd C:\Users\rhutu\source\repos\Game-Developer-Hackathon\UI
   npm install

Dev server with proxy (recommended)
- The project includes UI/proxy.conf.json which proxies /api -> https://localhost:51979
- Start dev server:
  npm run start
  # or
  npx ng serve --proxy-config proxy.conf.json --open
- Dev URL: http://localhost:4200
- Calls to /api/* will be forwarded to the backend (no CORS required)

Production build and serve from backend
1. Build frontend:
   cd UI
   npm run build:prod
   # output: UI/dist/football-ai-dashboard
2. Start backend (if not running). The backend includes logic to serve the built files from UI/dist/football-ai-dashboard (or UI/wwwroot) and will serve the app at:
   https://localhost:51979/

CI / Publish note
- UI/UI.csproj contains an MSBuild target to run npm install and ng build on PrepareForPublish and copy dist to wwwroot. Ensure Node is installed on CI agents.

Proxy verification (quick)
- With ng serve running and backend running, in another terminal run:
  curl -v "http://localhost:4200/api/matchintelligence/generate-events?count=1"
- If proxied correctly you will get the same response as:
  curl -k "https://localhost:51979/api/matchintelligence/generate-events?count=1"

Common issues & fixes
- 404 at http://localhost:4200/api/…
  - Ensure ng serve was started with proxy config from the UI folder:
	cd UI
	npx ng serve --proxy-config proxy.conf.json
- Node not found / npx fails
  - Install Node from https://nodejs.org or use nvm-windows and restart terminal
- SSL / dev cert issues proxying to https backend
  - Run: dotnet dev-certs https --trust and set proxy.conf.json secure:false (already set)
- Wrong backend port
  - Check Game-Developer-Hackathon/Properties/launchSettings.json and update UI/proxy.conf.json if needed

Repository layout
- Game-Developer-Hackathon/               (backend project)
  - Program.cs
  - Controllers/
  - Services/
  - Models/
  - Properties/launchSettings.json (applicationUrl)
- UI/                                     (Angular frontend)
  - package.json (scripts: start, build, build:prod)
  - proxy.conf.json
  - angular.json (outputPath: dist/football-ai-dashboard)
  - src/ (Angular sources)

If you want
- Add a root-level run script or PowerShell helper to start both backend and frontend
- Add a small preflight script to UI/package.json to ensure Node is present
- Add CI steps for building UI prior to publishing backend

