using Ada.ListadeTarefas.Data;
using Ada.ListadeTarefas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Ada.ListadeTarefas.Pages
{
    
    public class IndexModel : PageModel
    {
        private readonly TodoContext _context;

        public IndexModel(TodoContext context)
        {
            _context = context;
        }

        public IList<TodoItem> TodoItems { get; set; } = new List<TodoItem>();

        [BindProperty]
        public TodoItem NewTodo { get; set; } = new TodoItem();

        public async Task OnGetAsync()
        {
            TodoItems = await _context.TodoItems
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                TodoItems = await _context.TodoItems
                    .OrderByDescending(t => t.CreatedAt)
                    .ToListAsync();
                return Page();
            }

            NewTodo.CreatedAt = DateTime.Now;
            _context.TodoItems.Add(NewTodo);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostToggleCompleteAsync(int id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem != null)
            {
                todoItem.IsCompleted = !todoItem.IsCompleted;
                todoItem.CompletedAt = todoItem.IsCompleted ? DateTime.Now : null;
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem != null)
            {
                _context.TodoItems.Remove(todoItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}
