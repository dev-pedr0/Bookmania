using Bookmania.Data;
using Bookmania.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Bookmania.Pages.Livros
{
    [Authorize(Roles = "Funcionario,Gerente,Administrador")]
    public class EditarModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditarModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Livro Livro { get; set; } = new();

        [BindProperty]
        public List<int> SelectedTemas { get; set; } = new();

        public List<SelectListItem> TemasSelectList { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Livro = await _context.Livros
                .Include(l => l.Temas)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (Livro == null)
                return NotFound();

            SelectedTemas = Livro.Temas.Select(t => t.Id).ToList();

            TemasSelectList = await _context.Temas
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.Nome
                })
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                TemasSelectList = await _context.Temas
                    .Select(t => new SelectListItem
                    {
                        Value = t.Id.ToString(),
                        Text = t.Nome
                    })
                    .ToListAsync();

                return Page();
            }

            var livroExistente = await _context.Livros
                .Include(l => l.Temas)
                .FirstOrDefaultAsync(l => l.Id == Livro.Id);

            if (livroExistente == null)
                return NotFound();

            livroExistente.Titulo = Livro.Titulo;
            livroExistente.Autor = Livro.Autor;
            livroExistente.Editora = Livro.Editora;
            livroExistente.Paginas = Livro.Paginas;
            livroExistente.Quantidade = Livro.Quantidade;
            livroExistente.Produzido = Livro.Produzido;
            livroExistente.LivroCotacaoEspecial = Livro.LivroCotacaoEspecial;
            livroExistente.PrecoCompra = Livro.PrecoCompra;
            livroExistente.PrecoAluguel = Livro.PrecoAluguel;

            livroExistente.Temas.Clear();
            var novosTemas = await _context.Temas.Where(t => SelectedTemas.Contains(t.Id)).ToListAsync();
            foreach (var tema in novosTemas)
            {
                livroExistente.Temas.Add(tema);
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("/Livros/Index");
        }
    }
}
