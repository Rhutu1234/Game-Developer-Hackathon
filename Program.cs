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

app.UseCors("AllowAngular");
app.UseAuthorization();
app.MapControllers();
app.Run();
