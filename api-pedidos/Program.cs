using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var pedidos = new List<Pedido>();

app.MapPost("/orders", (Pedido pedido) =>
{
    pedido.Id = Guid.NewGuid().ToString();
    pedidos.Add(pedido);
    return Results.Created($"/orders/{pedido.Id}", pedido);
});

app.MapGet("/orders", () => Results.Ok(pedidos));

app.MapGet("/orders/{id}", (string id) =>
{
    var pedido = pedidos.FirstOrDefault(p => p.Id == id);
    return pedido != null ? Results.Ok(pedido) : Results.NotFound("Pedido não encontrado");
});

app.MapPut("/orders/{id}", (string id, Pedido updated) =>
{
    var pedido = pedidos.FirstOrDefault(p => p.Id == id);
    if (pedido == null) return Results.NotFound("Pedido não encontrado");

    pedido.Customer = updated.Customer;
    pedido.Items = updated.Items;
    pedido.Total = updated.Total;
    return Results.Ok(pedido);
});

app.MapDelete("/orders/{id}", (string id) =>
{
    var pedido = pedidos.FirstOrDefault(p => p.Id == id);
    if (pedido == null) return Results.NotFound("Pedido não encontrado");

    pedidos.Remove(pedido);
    return Results.Ok(pedido);
});

app.Run();