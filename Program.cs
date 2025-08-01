using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using AIBridges.Attributes;
using AIBridges.Data;
using AIBridges.Models;
using AIBridges.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add EF Core services to the container.
builder.Services.AddDbContext<AIBridgesDbContext>(options =>
{
    if (string.Equals(bool.TrueString, builder.Configuration["USE_SQL"]?.Trim(), StringComparison.OrdinalIgnoreCase))
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
    else 
    {
        options.UseInMemoryDatabase("AIBridges");
    }
});

// Add and http factory service to the container.
builder.Services.AddHttpClient("AIBridgesClient", client =>
{

});

// Add services to the container.
builder.Services.AddSingleton<AzureOpenAI>();
builder.Services.AddSingleton<AzureVisionV1>();
builder.Services.AddSingleton<AzureVisionV2>();
builder.Services.AddSingleton<Sonet3Dot5AI>();
builder.Services.AddSingleton<BitNetAI>();
builder.Services.AddSingleton<CustomOnnxAI>();
builder.Services.AddSingleton<Florence2AI>();

var app = builder.Build();

// Define constants for repeated route strings
const string ModelByIdRoute = "/api/config/models/{id}";

// Define constants for repeated error messages
const string ModelNotFoundMessage = "Model not found.";

const string AIBridgesServiceHelp = @"AIBridges Service is running! 

You can use the following endpoints:
- GET /api/config/models: Get all AI models.
- GET /api/config/models/{id}: Get AI model by ID.
- POST /api/config/models: Create a new AI model.
- PUT /api/config/models/{id}: Update an existing AI model.
- PATCH /api/config/models/{id}: Partially update an existing AI model.
- DELETE /api/config/models/{id}: Delete an AI model.
- POST /api/{modelName}/{version}/{actionName}: Execute an AI model action.

For more information, visit https://github.com/ErabliereApi/AIBridges

";

app.MapGet("/", () => AIBridgesServiceHelp);

app.MapPost("/api/{modelName}/{version}/{actionName}", async (string modelName, string version, string actionName, HttpRequest request) =>
{
    var db = request.HttpContext.RequestServices.GetRequiredService<AIBridgesDbContext>();

    var models = await db.Models.Where(m => m.Name == modelName).ToListAsync();
    if (models.Count == 0)
    {
        return Results.NotFound($"Model '{modelName}' not found.");
    }

    models = models.Where(m => m.Versions.Contains(version)).ToList();
    if (models.Count == 0)
    {
        return Results.NotFound($"Model '{modelName}' with version '{version}' not found.");
    }

    var model = models[0];

    var type = Assembly.GetExecutingAssembly().GetTypes()
        .FirstOrDefault(t => t.Name == model.Type && t.GetInterfaces().Contains(typeof(IAIService)));

    if (type == null)
    {
        throw new InvalidProgramException($"Executed type instance for {model.Type} not found. Review the code of the application.");
    }

    var aiService = request.HttpContext.RequestServices.GetService(type) as IAIService;
    if (aiService == null)
    {
        return Results.NotFound($"Model '{modelName}' with version '{version}' not found.");
    }

    await aiService.InitializeAsync();

    var response = await aiService.ProcessRequestAsync(new AIBridgeRequest
    {
        Task = actionName,
    }, request);

    return Results.Ok(response);
});

app.MapGet("/api/config/models", (AIBridgesDbContext context) =>
{
    return context.Models.ToList();
});

app.MapGet(ModelByIdRoute, (AIBridgesDbContext context, Guid id) =>
{
    var model = context.Models.Find(id);
    if (model == null)
    {
        return Results.NotFound(ModelNotFoundMessage);
    }
    return Results.Ok(model);
});

app.MapPost("/api/config/models", async (AIBridgesDbContext context, HttpRequest request) =>
{
    var model = await JsonSerializer.DeserializeAsync<AIModel>(request.Body);
    if (model == null)
    {
        return Results.BadRequest("Invalid model data.");
    }
    context.Models.Add(model);
    await context.SaveChangesAsync();
    return Results.Created($"/api/config/models/{model.Id}", model);
});

app.MapPut(ModelByIdRoute, async (AIBridgesDbContext context, Guid id, HttpRequest request) =>
{
    var model = await context.Models.FindAsync(id);
    if (model == null)
    {
        return Results.NotFound(ModelNotFoundMessage);
    }
    var updatedModel = await JsonSerializer.DeserializeAsync<AIModel>(request.Body);
    if (updatedModel == null)
    {
        return Results.BadRequest("Invalid model data.");
    }
    model.Name = updatedModel.Name;
    model.Description = updatedModel.Description;
    model.Versions = updatedModel.Versions;
    model.ActionNames = updatedModel.ActionNames;
    model.Type = updatedModel.Type;
    model.Endpoint = updatedModel.Endpoint;
    model.Key = updatedModel.Key;
    await context.SaveChangesAsync();
    return Results.Ok(model);
});

app.MapPatch(ModelByIdRoute, async (AIBridgesDbContext context, Guid id, HttpRequest request) =>
{
    var model = await context.Models.FindAsync(id);
    if (model == null)
    {
        return Results.NotFound(ModelNotFoundMessage);
    }
    var updatedModel = await JsonSerializer.DeserializeAsync<AIModel>(request.Body);
    if (updatedModel == null)
    {
        return Results.BadRequest("Invalid model data.");
    }
    model.Name = updatedModel.Name ?? model.Name;
    model.Description = updatedModel.Description ?? model.Description;
    model.Versions = updatedModel.Versions ?? model.Versions;
    model.ActionNames = updatedModel.ActionNames ?? model.ActionNames;
    model.Type = updatedModel.Type ?? model.Type;
    model.Endpoint = updatedModel.Endpoint ?? model.Endpoint;
    model.Key = updatedModel.Key ?? model.Key;
    await context.SaveChangesAsync();
    return Results.Ok(model);
});

app.MapDelete(ModelByIdRoute, async (AIBridgesDbContext context, Guid id) =>
{
    var model = await context.Models.FindAsync(id);
    if (model == null)
    {
        return Results.NotFound(ModelNotFoundMessage);
    }
    context.Models.Remove(model);
    await context.SaveChangesAsync();
    return Results.NoContent();
});

// Automatically apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AIBridgesDbContext>();

    if (dbContext.Database.IsSqlServer()) 
    {
        await dbContext.Database.MigrateAsync();
    }

    var supportedModels = Assembly.GetAssembly(typeof(IAIService))?.GetTypes()
        .Where(t => t.IsClass && t.GetInterfaces().Contains(typeof(IAIService)))
        .Select(t => scope.ServiceProvider.GetRequiredService(t))
        .ToList();

    foreach (var model in supportedModels ?? throw new InvalidOperationException("No AI models found. Ensure that AI model classes implement IAIService and are registered in the DI container."))
    {
        // Check if the model already exists in the database
        var existingModel = await dbContext.Models.FirstOrDefaultAsync(m => m.Name == model.GetType().Name);

        if (existingModel == null)
        {
            var modelEntity = new AIModel
            {
                Name = model.GetType().Name,
                Description = model.GetType().GetCustomAttribute<DescriptionAttribute>()?.Description ?? "",
                Versions = model.GetType().GetCustomAttribute<VersionAttribute>()?.Version ?? "1.0",
                ActionNames = "",
                Type = model.GetType().Name,
                Endpoint = "",
                Key = ""
            };

            // Add the new model to the database
            dbContext.Models.Add(modelEntity);
        }
        else
        {
            existingModel.Description = model.GetType().GetCustomAttribute<DescriptionAttribute>()?.Description ?? "";
            existingModel.Versions = model.GetType().GetCustomAttribute<VersionAttribute>()?.Version ?? "";
        }
    }

    await dbContext.SaveChangesAsync();
}

await app.RunAsync();
