using Mansari.Store.Basket.API.Middlewares;
using Mansari.Store.Basket.Application;
using Mansari.Store.Basket.Infrastructure;
using Microex.Swagger.Application;
using Microex.Swagger.SwaggerGen.Application;
using Microex.Swagger.SwaggerUI;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var problemDetails = new ValidationProblemDetails(context.ModelState)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "One or more validation errors occurred."
            };

            return new BadRequestObjectResult(problemDetails);
        };
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI3(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Basket API v1");
    });
}

app.MapHealthChecks("/health");

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
