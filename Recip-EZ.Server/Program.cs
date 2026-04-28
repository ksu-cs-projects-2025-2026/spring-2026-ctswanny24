using Recip_EZ.Server.Data;
using Microsoft.EntityFrameworkCore;
using CsvHelper.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


builder.Services.AddDbContext<RecipEzDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



builder.Services.AddScoped<Recip_EZ.Server.Services.UserService>();
builder.Services.AddScoped<Recip_EZ.Server.Services.RecipeService>();
builder.Services.AddScoped<Recip_EZ.Server.Services.InventoryService>();
builder.Services.AddScoped<Recip_EZ.Server.Services.IngredientAliasService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<RecipEzDbContext>();

    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();

    var seeder = new DbSeeder(context);

    seeder.Seed();
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<RecipEzDbContext>();
    var ingredientAliasService = scope.ServiceProvider.GetRequiredService<Recip_EZ.Server.Services.IngredientAliasService>();

    var seeder = new DbSeeder(context);
    //seeder.Seed();
    ingredientAliasService.AddToAliases();
}

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
