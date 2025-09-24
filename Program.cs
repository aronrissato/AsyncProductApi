using AsyncProductApi.Data;
using AsyncProductApi.Dtos;
using AsyncProductApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite("Data Source=RequestDB.db;"));

var app = builder.Build();

app.UseHttpsRedirection();

// Start endpoint
app.MapPost("api/v1/products", async (AppDbContext dbContext, ListingRequest request) =>
{
    request.RequestStatus = "Accept";
    request.EstimatedCompletionTime = "2025-08-22:14:00:00";

    await dbContext.ListingRequests.AddAsync(request);
    await dbContext.SaveChangesAsync();

    return Results.Accepted($"api/v1/productstatus/{request.RequestId}", request);
});

// Status endpoint
app.MapGet("api/v1/productstatus/{requestId}", (AppDbContext context, string requestId) =>
{
    var request = context.ListingRequests.FirstOrDefault(x => x.RequestId == requestId);
    if (request == null)
        return Results.NotFound();

    ListingStatus listingStatus = new()
    {
        RequestStatus = request.RequestStatus,
        ResourceUrl = string.Empty
    };

    if (request.RequestStatus!.ToUpper() == "COMPLETE")
    {
        listingStatus.ResourceUrl = $"api/v1/products/{Guid.NewGuid().ToString()}";
        return Results.Ok(request);
    }

    listingStatus.EstimatedCompletionTime = "2025-08-22:14:00:00";
    return Results.Ok(request);
});

app.Run();