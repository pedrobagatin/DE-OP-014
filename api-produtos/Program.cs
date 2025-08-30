using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Net;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Banco de dados em memória (Thread-safe)
var db = new ConcurrentDictionary<string, Product>();

// Validação manual (opcional, pode usar FluentValidation)
app.MapGet("/products", () => Results.Ok(db.Values));

app.MapGet("/products/{id}", (string id) =>
{
    if (db.TryGetValue(id, out var product))
        return Results.Ok(product);
    return Results.NotFound(new { error = "Produto não encontrado" });
});

app.MapPost("/products", (CreateProductRequest request) =>
{
    var product = new Product(
        Id: Guid.NewGuid().ToString(),
        Name: request.Name,
        Price: request.Price,
        Quantity: request.Quantity
    );

    db[product.Id] = product;

    return Results.Created($"/products/{product.Id}", product);
});

app.MapPut("/products/{id}", (string id, UpdateProductRequest request) =>
{
    if (!db.TryGetValue(id, out var existing))
        return Results.NotFound(new { error = "Produto não encontrado" });

    var product = new Product(
        Id: id,
        Name: request.Name ?? existing.Name,
        Price: request.Price ?? existing.Price,
        Quantity: request.Quantity ?? existing.Quantity
    );

    db[id] = product;
    return Results.Ok(product);
});

app.MapDelete("/products/{id}", (string id) =>
{
    if (db.TryRemove(id, out _))
        return Results.NoContent();
    return Results.NotFound(new { error = "Produto não encontrado" });
});

app.Run();

// === Modelos ===

record Product(string Id, string Name, double Price, int Quantity);

record CreateProductRequest(
    [Required][StringLength(100)] string Name,
    [Range(0.01, double.MaxValue)] double Price,
    [Range(0, int.MaxValue)] int Quantity
);

record UpdateProductRequest(
    string? Name,
    double? Price,
    int? Quantity
);
