using Bookmania.Data;
using Bookmania.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookmania.Pages.Livros
{
    [Authorize(Roles = "Funcionario,Gerente,Administrador")]
    public class ExcluirModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ExcluirModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Livro Livro { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Livro = await _context.Livros
                .Include(l => l.Temas)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (Livro == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var livro = await _context.Livros
                .Include(l => l.Temas)
                .FirstOrDefaultAsync(l => l.Id == Livro.Id);

            if (livro == null)
            {
                return NotFound();
            }

            _context.Livros.Remove(livro);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Livros/Index");
        }
    }
}
