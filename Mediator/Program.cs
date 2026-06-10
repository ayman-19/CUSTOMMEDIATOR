using BenchmarkDotNet.Running;
using FluentValidation;
using Mediator.Commands.Add;
using Mediator.Pipelines;
using MediatR;
using MediatR.Pipeline;

namespace Mediator;

public class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		//builder.Services.AddMediatR(cfg =>
		//    cfg.RegisterServicesFromAssemblyContaining<AddCommandHandler>()
		//);
		builder.Services.AddMediatR(cfg =>
		{
			cfg.RegisterServicesFromAssemblyContaining<AddCommandHandler>();
			cfg.AddOpenBehavior(typeof(RequestPostProcessorBehavior<,>));
			cfg.AddOpenBehavior(typeof(RequestPreProcessorBehavior<,>));
		});
		builder.Services.AddTransient<IRequestPostProcessor<AddCommand, double>, AddPostPipeline>();
		builder.Services.AddScoped<IRequestPreProcessor<AddCommand>, AddPipeline>();
		builder.Services.AddValidatorsFromAssemblyContaining<AddCommandValidator>();

		builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
		builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
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
