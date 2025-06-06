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
    public class CriarModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CriarModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Livro Livro { get; set; } = new();

        [BindProperty]
        public List<int> SelectedTemas { get; set; } = new();

        public List<SelectListItem> TemasSelectList { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
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

            Livro.Temas = await _context.Temas
                .Where(t => SelectedTemas.Contains(t.Id))
                .ToListAsync();

            Livro.VerificarCotacaoEspecial();

            Livro.PrecoAluguel = Livro.LivroCotacaoEspecial ? 20 : 5;

            _context.Livros.Add(Livro);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Livros/Index");
        }
    }
}
