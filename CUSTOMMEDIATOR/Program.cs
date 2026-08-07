using CUSTOMMEDIATOR.Commands.Add;
using CUSTOMMEDIATOR.Interfaces;
using CUSTOMMEDIATOR.Notifications;

namespace CUSTOMMEDIATOR;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthorization();
        builder.Services.AddControllers();
        builder.Services.AddMediator();
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

        app.MapGet(
            "/api/v1/notify",
            async (IPublisher mediator) =>
            {
                await mediator.Publish(new PingNotification10());
                return Results.Ok();
            }
        );
        app.Run();
    }
}
