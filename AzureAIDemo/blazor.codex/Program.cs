using System.Text.Json;
using blazor.codex.Models;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseStaticFiles();
app.UseDefaultFiles();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var engine = new CodexEngine();


var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapPost("/codegen", async ([FromBody]paramobj jsonstring) =>
{
    System.Console.WriteLine($"Received natural language command: '{jsonstring}'");
    var response = await engine.getCompletion(jsonstring.text);
    return JsonSerializer.Serialize(response);
});

app.MapGet("/reset", () => {
    engine.context.resetContext();
    var context =  engine.context.getContext();
    return JsonSerializer.Serialize(new {
        context
    });
});

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
       new WeatherForecast
       (
           DateTime.Now.AddDays(index),
           Random.Shared.Next(-20, 55),
           summaries[Random.Shared.Next(summaries.Length)]
       ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

/*
 
// Gets natural language and returns code
app.post("/codegen", async (req, res) => {
    console.log(`Received natural language command: '${req.body.text}'`);
    const response = await getCompletion(req.body.text);
    res.send(JSON.stringify(response));
});

// Gets natural language and returns code
app.get("/reset", async (_req, res) => {
    context.resetContext();
    res.send(
        JSON.stringify({
            context: context.getContext()
        })
    );
});
 */

public class paramobj
{
    public string text { get; set; }
}