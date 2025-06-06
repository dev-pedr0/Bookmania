using Bookmania.Data;
using Bookmania.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookmania.Pages.ListasEsperas
{
    [Authorize(Roles = "Funcionario,Gerente,Administrador")]
    public class PorLivroModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public PorLivroModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Livro Livro { get; set; } = default!;
        public List<ListaEspera> Espera { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int LivroId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Livro = await _context.Livros
                .Include(l => l.Temas)
                .FirstOrDefaultAsync(l => l.Id == LivroId);

            if (Livro == null)
                return NotFound();

            Espera = await _context.ListaEspera
                .Include(e => e.Usuario)
                .Where(e => e.LivroId == LivroId)
                .OrderBy(e => e.DataSolicitacao)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostRemoverAsync(int id)
        {
            var entrada = await _context.ListaEspera.FindAsync(id);

            if (entrada != null)
            {
                _context.ListaEspera.Remove(entrada);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage(new { LivroId });
        }
    }
}
