using Microsoft.EntityFrameworkCore;
using WildlifeConservation.Api;
using WildlifeConservation.Api.Middleware;
using WildlifeConservation.Repositories;
using WildlifeConservation.Repositories.Data;
using WildlifeConservation.Services;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// CORS (DEV only config)
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DI
builder.Services.AddRepositoryLayer(builder.Configuration);
builder.Services.AddServiceLayer();
builder.Services.AddAutoMapper(_ => { }, typeof(ApiAssemblyMarker).Assembly);

var app = builder.Build();

// pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

app.UseHttpsRedirection();

// IMPORTANT ORDER
app.UseRouting();

app.UseCors("DevCors");

app.UseAuthorization();

app.UseMiddleware<ServiceExceptionMiddleware>();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<WildlifeDbContext>();
    db.Database.Migrate();
}

app.Run();