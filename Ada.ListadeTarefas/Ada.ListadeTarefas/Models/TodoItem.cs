using System.ComponentModel.DataAnnotations;

namespace Ada.ListadeTarefas.Models;

//public class TodoItem
//{
//    public int Id { get; set; }
//    public string Title { get; set; } = string.Empty;
//    public string Description { get; set; } = string.Empty;
//    public bool IsCompleted { get; set; }
//    public DateTime CreatedAt { get; set; } = DateTime.Now;
//    public DateTime? CompletedAt { get; set; }
//}
public class TodoItem
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O título é obrigatório")]
    [StringLength(200, ErrorMessage = "O título não pode exceder 200 caracteres")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "A descrição não pode exceder 1000 caracteres")]
    public string? Description { get; set; }

    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? CompletedAt { get; set; }
}