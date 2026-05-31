using CUSTOMMEDIATOR.Commands.Add;
using CUSTOMMEDIATOR.Interfaces;

namespace CUSTOMMEDIATOR;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthorization();
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();
        //builder.Services.AddValidatorsFromAssemblyContaining<Program>();
        builder.Services.AddMediator(typeof(Program).Assembly);
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();

        app.UseAuthorization();
        app.MapControllers();

        app.MapPost(
            "/api/v1/add",
            async (IMediator mediator, AddCommand command) =>
            {
                var response = await mediator.Send(command);
                return Results.Ok(response);
            }
        );

        app.MapPost(
            "/api/v2/add",
            async (IMediator mediator, AddCommand command) =>
            {
                var response = await mediator.Send<AddCommand, double>(command);
                return Results.Ok(response);
            }
        );

        app.Run();
    }
}
