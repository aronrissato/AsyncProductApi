using AsyncProductApi.Data;
using AsyncProductApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(
    opt => 
        opt.UseSqlite("Data Source=RequestDB.db;"));

var app = builder.Build();

app.UseHttpsRedirection();

// Start endpoint
app.MapPost("api/v1/products", async (AppDbContext dbContext, ListingRequest request) =>
{
    if (request == null)
        return Results.BadRequest();
    
    request.RequestStatus = "Accept";
    request.EstimatedCompletionTime = "2025-08-22:14:00:00";
    
    await dbContext.ListingRequests.AddAsync(request);
    await dbContext.SaveChangesAsync();
    
    return Results.Accepted($"api/v1/productStatus/{request.RequestId}", request);
});

app.Run();
