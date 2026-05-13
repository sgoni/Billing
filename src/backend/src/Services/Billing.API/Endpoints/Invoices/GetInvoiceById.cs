namespace Billing.API.Endpoints.Invoices;

//- Accepts a invoice ID.
//- Uses a GetInvoiceByIdQuery to fetch orders.
//- Returns the list of orders for that customer.m

//public record GetInvoiceByIdRequest(Guid Id);

public record GetInvoiceByIdResponse(InvoiceDto Invoice);

public class GetApInvoiceById : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/billings/{id}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new GetInvoiceByIdQuery(id));

                var response = result.Adapt<GetInvoiceByIdResponse>();

                return Results.Ok(response);
            })
            .WithName("GetInvoiceById")
            .Produces<GetInvoiceByIdResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get a specific invoice by id.")
            .WithDescription("Get a specific invoice by id.");
    }
}