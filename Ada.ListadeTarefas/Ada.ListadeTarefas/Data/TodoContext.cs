using Ada.ListadeTarefas.Models;
using Microsoft.EntityFrameworkCore;

namespace Ada.ListadeTarefas.Data;

public class TodoContext : DbContext
{
    public TodoContext(DbContextOptions<TodoContext> options) : base(options)
    {
    }

    public DbSet<TodoItem> TodoItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
        });

        // Se estiver usando InMemory, precisa configurar explicitamente
        if (Database.IsInMemory())
        {
            modelBuilder.Entity<TodoItem>().Property(e => e.CreatedAt).HasDefaultValue(DateTime.Now);
        }
    }
}