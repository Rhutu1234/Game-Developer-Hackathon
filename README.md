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

AI provider configuration

The backend reads AI provider settings from the root `appsettings.json` file:

| Setting | Purpose |
| --- | --- |
| `AzureOpenAI:Endpoint` | Your Azure OpenAI resource endpoint, such as `https://<resource>.openai.azure.com/`. |
| `AzureOpenAI:ApiKey` | The Azure OpenAI API key. Leave the sample placeholder or blank to use Ollama instead. |
| `AzureOpenAI:DeploymentName` | The Azure OpenAI deployment name (not necessarily the underlying model name). |
| `Ollama:Endpoint` | The local Ollama server URL. The default is `http://localhost:11434`. |
| `Ollama:Model` | The model name that Ollama has downloaded, for example `llama3.2`. |
| `Logging:LogLevel` | The minimum log level for the application and ASP.NET Core. |
| `AllowedHosts` | The hosts accepted by ASP.NET Core; `*` allows any host. |
| `CorsSettings:AllowedOrigins` | The configured frontend origins. The current `Program.cs` CORS policy allows any origin, so this setting is not currently applied. |

To use Azure OpenAI, replace the sample endpoint, key, and deployment name. Avoid committing a real API key: set it in the process environment instead, for example in PowerShell:

```powershell
$env:AzureOpenAI__ApiKey = "your-azure-openai-api-key"
dotnet run --project Game-Developer-Hackathon.csproj
```

To use Ollama locally when Azure OpenAI is not configured or cannot be reached:

1. Install Ollama from [ollama.com](https://ollama.com/).
2. Start the local server if it is not already running. Keep this terminal open:

   ```powershell
   ollama serve
   ```

3. In another terminal, download the configured model (or change `Ollama:Model` in `appsettings.json` to another model you have pulled):

   ```powershell
   ollama pull llama3.2
   ```

4. Keep `Ollama:Endpoint` set to `http://localhost:11434` and set `Ollama:Model` to `llama3.2`. Leave `AzureOpenAI:ApiKey` as `YOUR_AZURE_OPENAI_API_KEY` (or blank) to select Ollama directly.
5. Run the backend as described above. If an Azure request fails, the service also tries Ollama automatically. If neither AI provider is available, it returns its built-in local analysis response.

You can check that Ollama is serving requests at `http://localhost:11434` and that the model is available with:

```powershell
ollama list
```

Repository layout
- Game-Developer-Hackathon/               (backend project)
  - Program.cs
  - Controllers/
  - Services/
  - Models/
  - Properties/launchSettings.json (applicationUrl)

If you want
- Add a root-level run script or PowerShell helper to start both backend and frontend
- Add a small preflight script to UI/package.json to ensure Node is present
- Add CI steps for building UI prior to publishing backend
