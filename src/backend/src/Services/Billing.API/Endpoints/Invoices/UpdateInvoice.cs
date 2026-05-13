namespace Billing.API.Endpoints.Invoices;

//- Accepts a UpdateApInvoiceRequest.
//- Maps the request to an UpdateApInvoiceCommand.
//- Sends the command for processing.
//- Returns a success or error response based on the outcome.

public record UpdateInvoiceRequest(InvoiceDto Invoice);

public record UpdateInvoiceResponse(bool IsSuccess);

public class UpdateInvoice : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/billings", async (UpdateInvoiceRequest request, ISender sender) =>
            {
                var command = request.Adapt<UpdatelInvoiceCommand>();

                var result = await sender.Send(command);

                var response = result.Adapt<UpdateInvoiceResponse>();

                return Results.Ok(response);
            })
            .WithName("UpdateInvoice")
            .Produces<UpdateInvoiceResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Update invoice (if not approved/paid)")
            .WithDescription("Update invoice (if not approved/paid)t");
    }
}