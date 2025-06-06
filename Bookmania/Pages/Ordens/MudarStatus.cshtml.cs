using Bookmania.Data;
using Bookmania.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookmania.Pages.Ordens
{
    [Authorize(Roles = "Funcionario,Gerente,Administrador")]
    public class MudarStatusModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public MudarStatusModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Ordem Ordem { get; set; }

        [BindProperty]
        public StatusOrdem NovoStatus { get; set; }

        public List<StatusOrdem> StatusDisponiveis { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Ordem = await _context.Ordens
                .Include(o => o.Usuario)
                .Include(o => o.Itens).ThenInclude(i => i.Livro)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (Ordem == null)
            {
                return NotFound();
            }

            DefinirStatusDisponiveis();
            NovoStatus = Ordem.Status; // default no dropdown é o status atual

            return Page();
        }

        private void DefinirStatusDisponiveis()
        {
            if (Ordem.Tipo == TipoOrdem.Aluguel)
            {
                StatusDisponiveis = new List<StatusOrdem>
                {
                    StatusOrdem.Confirmada,
                    StatusOrdem.Atrasada,
                    StatusOrdem.Cancelada,
                    StatusOrdem.Concluida
                };
            }
            else if (Ordem.Tipo == TipoOrdem.Compra)
            {
                StatusDisponiveis = new List<StatusOrdem>
                {
                    StatusOrdem.Cancelada,
                    StatusOrdem.Concluida
                };
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var ordemDb = await _context.Ordens.FindAsync(id);

            if (ordemDb == null)
            {
                return NotFound();
            }

            ordemDb.Status = NovoStatus;
            await _context.SaveChangesAsync();

            return RedirectToPage("/Ordens/Index");
        }
    }
}
