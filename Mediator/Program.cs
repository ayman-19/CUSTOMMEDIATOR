using BenchmarkDotNet.Running;
using FluentValidation;
using Mediator.Commands.Add;
using MediatR;

namespace Mediator;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<AddCommandHandler>()
        );
        builder.Services.AddValidatorsFromAssemblyContaining<AddCommandValidator>();

        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        builder.Services.AddAuthorization();

        builder.Services.AddOpenApi();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }
        app.MapPost(
            "/api/v1/add",
            async (IMediator mediator, AddCommand command) =>
            {
                var response = await mediator.Send(command);
                return Results.Ok(response);
            }
        );
        BenchmarkRunner.Run<MediatorBenchmark>();
        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.Run();
    }
}
