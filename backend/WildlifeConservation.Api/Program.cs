using System.Text.Json.Serialization;
using WildlifeConservation.Api;
using WildlifeConservation.Api.Middleware;
using WildlifeConservation.Repositories;
using WildlifeConservation.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRepositoryLayer(builder.Configuration);
builder.Services.AddServiceLayer();
builder.Services.AddAutoMapper(_ => { }, typeof(ApiAssemblyMarker).Assembly);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ServiceExceptionMiddleware>();

app.MapControllers();

app.Run();
