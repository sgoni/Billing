namespace Billing.API.Endpoints.Invoices;

//- Accepts pagination parameters.
//- Constructs a GetInvoicesQuery with these parameters.
//- Retrieves the data and returns it in a paginated format.

//public record GetInvoicesRequest(PaginationRequest PaginationRequest);
public record GetInvoicesResponse(PaginatedResult<InvoiceDto> Invoices);

public class GetInvoices : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/billings", async ([AsParameters] PaginationRequest request, ISender sender) =>
            {
                var result = await sender.Send(new GetInvoicesQuery(request));

                var response = result.Adapt<GetInvoicesResponse>();

                return Results.Ok(response);
            })
            .WithName("GetInvoiceList")
            .Produces<GetInvoicesResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get invoice list")
            .WithDescription("Get invoice list");
    }
}