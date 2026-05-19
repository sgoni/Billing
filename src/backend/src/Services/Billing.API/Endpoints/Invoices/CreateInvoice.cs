namespace Billing.API.Endpoints.Invoices;

//- Accepts a CreateInvoiceRequest object.
//- Maps the request to a CreateInvoiceCommand.
//- Uses MediatR to send the command to the corresponding handler.
//- Returns a response with the created Invoice ID.

public record CreateInvoiceRequest(InvoiceDto Invoice);

public record CreateInvoiceResponse(Guid Id);

public class CreateInvoice : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/billings", async (CreateInvoiceRequest request, ISender sender) =>
                {
                    var command = request.Adapt<CreateInvoiceCommand>();

                    var result = await sender.Send(command);

                    var response = result.Adapt<CreateInvoiceResponse>();

                    return Results.Created($"/billings/{response.Id}", response);
                }
            )
            .WithName("CreateInvoice")
            .Produces<CreateInvoiceResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create invoice")
            .WithDescription("Create invoice");
    }
}