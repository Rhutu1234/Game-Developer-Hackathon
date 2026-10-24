using Game_Developer_Hackathon.Services;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Extensions.FileProviders;
using System.IO;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Services
builder.Services.AddScoped<ISyntheticDataService, SyntheticDataService>();
builder.Services.AddScoped<ITacticalAgentService, TacticalAgentService>();

// CORS Policy for Angular Frontend
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAngular", policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Serve Angular frontend static files from the UI project's wwwroot or dist folder when present
var uiRoot = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "UI"));
var uiStaticPath = Path.Combine(uiRoot, "wwwroot");

if (!Directory.Exists(uiStaticPath))
{
    // fallback to dist if project built differently
    var distPath = Path.Combine(uiRoot, "dist");

    // Common Angular configuration uses outputPath "dist/<project-name>" (see angular.json)
    var namedDist = Path.Combine(distPath, "football-ai-dashboard");
    if (Directory.Exists(namedDist))
    {
        uiStaticPath = namedDist;
    }
    else if (Directory.Exists(distPath))
    {
        // If dist contains a single subfolder, serve it
        var subdirs = Directory.GetDirectories(distPath);
        if (subdirs.Length == 1)
        {
            uiStaticPath = subdirs[0];
        }
        else
        {
            uiStaticPath = distPath; // may contain index.html at top level
        }
    }
    else
    {
        uiStaticPath = distPath; // default fallback
    }
}

if (Directory.Exists(uiStaticPath))
{
    var fileProvider = new PhysicalFileProvider(uiStaticPath);
    app.UseDefaultFiles(new Microsoft.AspNetCore.Builder.DefaultFilesOptions { FileProvider = fileProvider });
    app.UseStaticFiles(new Microsoft.AspNetCore.Builder.StaticFileOptions { FileProvider = fileProvider });

    // Map fallback to index.html for SPA routes
    app.MapFallback(async context =>
    {
        var indexPath = Path.Combine(uiStaticPath, "index.html");
        if (File.Exists(indexPath))
        {
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync(indexPath);
            return;
        }

        context.Response.StatusCode = 404;
    });
}

app.UseCors("AllowAngular");
app.UseAuthorization();
app.MapControllers();
app.Run();
