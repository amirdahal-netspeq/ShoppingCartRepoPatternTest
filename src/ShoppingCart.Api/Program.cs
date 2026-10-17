using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using ShoppingCart.Api.Middleware;
using ShoppingCart.Application;
using ShoppingCart.Application.Common;
using ShoppingCart.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
            .Where(e => e.Value is { Errors.Count: > 0 })
            .ToDictionary(e => e.Key, e => e.Value!.Errors.Select(x => x.ErrorMessage).ToArray());

            return new BadRequestObjectResult(
                ApiResponseDto<object>.Error("One or more validation errors occurred.", errors));
        });


builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseExceptionHandler();  // first, so it wraps everything after it
app.UseStatusCodePages(async context =>  // gives bare responses (e.g. a 404 from routing) a ProblemDetails body
{
    var response = context.HttpContext.Response;
    await response.WriteAsJsonAsync(ApiResponseDto<object>.Error(ReasonPhrases.GetReasonPhrase(response.StatusCode)));
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    await app.Services.MigrateDatabaseAsync();  // development convenience only
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
