using Ada.ListadeTarefas.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// Configurar Entity Framework com fallback
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (!string.IsNullOrEmpty(connectionString))
{
    try
    {
        builder.Services.AddDbContext<TodoContext>(options =>
            options.UseSqlServer(connectionString));

        // Testar conexão
        using var scope = builder.Services.BuildServiceProvider().CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TodoContext>();
        context.Database.CanConnect(); // Testa a conexão

        Console.WriteLine("Usando banco de dados SQL Server");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Falha ao conectar ao SQL Server: {ex.Message}");
        Console.WriteLine("Usando banco de dados em memória como fallback.");
        builder.Services.AddDbContext<TodoContext>(options =>
            options.UseInMemoryDatabase("TodoDb"));
    }
}
else
{
    Console.WriteLine("Connection string não configurada. Usando banco de dados em memória.");
    builder.Services.AddDbContext<TodoContext>(options =>
        options.UseInMemoryDatabase("TodoDb"));
}

var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TodoContext>();
    try
    {
        context.Database.EnsureCreated();
        Console.WriteLine("Banco de dados inicializado com sucesso.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao inicializar banco de dados: {ex.Message}");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
