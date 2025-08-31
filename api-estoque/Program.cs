using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var produtos = new List<Produto>();

app.MapPost("/products", (Produto produto) =>
{
    produto.Id = Guid.NewGuid().ToString();
    produtos.Add(produto);
    return Results.Created($"/products/{produto.Id}", produto);
});

app.MapGet("/products", () => Results.Ok(produtos));

app.MapGet("/products/{id}", (string id) =>
{
    var produto = produtos.FirstOrDefault(p => p.Id == id);
    return produto != null ? Results.Ok(produto) : Results.NotFound("Produto não encontrado");
});

app.MapPut("/products/{id}", (string id, Produto updated) =>
{
    var produto = produtos.FirstOrDefault(p => p.Id == id);
    if (produto == null) return Results.NotFound("Produto não encontrado");

    produto.Name = updated.Name;
    produto.Quantity = updated.Quantity;
    produto.Price = updated.Price;
    return Results.Ok(produto);
});

app.MapDelete("/products/{id}", (string id) =>
{
    var produto = produtos.FirstOrDefault(p => p.Id == id);
    if (produto == null) return Results.NotFound("Produto não encontrado");

    produtos.Remove(produto);
    return Results.Ok(produto);
});

app.Run();