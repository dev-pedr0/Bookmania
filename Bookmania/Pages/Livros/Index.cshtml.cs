using Bookmania.Data;
using Bookmania.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookmania.Pages.Livros
{
    [Authorize(Roles = "Funcionario,Gerente,Administrador")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Livro> Livros { get; set; } = new List<Livro>();
        public List<string> Temas { get; set; } = new List<string>();

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Tema { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? OrderBy { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool ApenasZerados { get; set; }

        public async Task OnGetAsync()
        {
            var query = _context.Livros
                .Include(l => l.Temas)
                .AsQueryable();

            Temas = await _context.Temas
                .Select(t => t.Nome)
                .Distinct()
                .ToListAsync();

            if (!string.IsNullOrEmpty(SearchString))
            {
                query = query.Where(l => l.Titulo.Contains(SearchString));
            }

            if (!string.IsNullOrEmpty(Tema))
            {
                query = query.Where(l => l.Temas.Any(t => t.Nome == Tema));
            }

            if (ApenasZerados)
            {
                query = query.Where(l => l.Quantidade == 0);
            }

            query = OrderBy switch
            {
                "Preco" => query.OrderBy(l => l.PrecoCompra),
                _ => query.OrderBy(l => l.Titulo),
            };

            Livros = await query.ToListAsync();
        }
    }
}
