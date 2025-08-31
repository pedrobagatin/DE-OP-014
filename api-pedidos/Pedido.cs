public class Pedido
{
    public string Id { get; set; } = null!;
    public string Customer { get; set; } = null!;
    public List<string> Items { get; set; } = new();
    public decimal Total { get; set; }
}