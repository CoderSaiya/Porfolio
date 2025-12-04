using System.Reflection;
using BE_Portfolio.Configuration;
using BE_Portfolio.Persistence.Data;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo {
        Title = "Portfolio API", Version = "v1",
        Description = "API cho portfolio (.NET + MongoDB). Hỗ trợ upload ảnh WebP, Data URL, contact form."
    });

    // Bật [SwaggerOperation] annotations
    c.EnableAnnotations();

    // XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (System.IO.File.Exists(xmlPath)) c.IncludeXmlComments(xmlPath);
});

// builder.WebHost.ConfigureKestrel(options =>
// {
//     options.ListenAnyIP(8082);
// });

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://my-portfolio.nhatcuong.io.vn")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddSettings(builder.Configuration);
builder.Services.AddMongoDbContext(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var mongo = scope.ServiceProvider.GetRequiredService<IMongoDbContext>();
    // await mongo.EnsureIndexesAsync();
    // await mongo.EnsureSeedAsync();
    // await ImageSeeder.SeedProjectImagesAsync(scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
