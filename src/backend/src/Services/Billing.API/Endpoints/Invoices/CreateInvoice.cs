namespace Billing.API.Endpoints.Invoices;

//- Accepts a CreateInvoiceRequest object.
//- Maps the request to a CreateInvoiceCommand.
//- Uses MediatR to send the command to the corresponding handler.
//- Returns a response with the created Invoice ID.

public record CreateApInvoiceRequest(InvoiceDto ApInvoice);

public record CreateApInvoiceResponse(Guid Id);

public class CreateInvoice : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/billings", async (CreateApInvoiceRequest request, ISender sender) =>
                {
                    var command = request.Adapt<CreateInvoiceCommand>();

                    var result = await sender.Send(command);

                    var response = result.Adapt<CreateApInvoiceResponse>();

                    return Results.Created($"/billings/{response.Id}", response);
                }
            )
            .WithName("CreateInvoice")
            .Produces<CreateApInvoiceResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create invoice")
            .WithDescription("Create invoice");
    }
}