using Bookmania.Data;
using Bookmania.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookmania.Pages.Ordens
{
    [Authorize(Roles = "Funcionario,Gerente,Administrador")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Ordem> Ordens { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public TipoOrdem? Tipo { get; set; }

        [BindProperty(SupportsGet = true)]
        public StatusOrdem? Status { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            await AtualizarOrdensAtrasadas();

            var query = _context.Ordens
                .Include(o => o.Usuario)
                .Include(o => o.Itens).ThenInclude(i => i.Livro)
                .AsQueryable();

            if (Tipo.HasValue)
                query = query.Where(o => o.Tipo == Tipo);

            if (Status.HasValue)
                query = query.Where(o => o.Status == Status);

            Ordens = await query.OrderByDescending(o => o.Data).ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAtualizarAsync()
        {
            await AtualizarOrdensAtrasadas();
            return RedirectToPage();
        }

        private async Task AtualizarOrdensAtrasadas()
        {
            var hoje = DateTime.Today;

            var ordensAtrasadas = await _context.Ordens
                .Where(o => o.Tipo == TipoOrdem.Aluguel &&
                            (o.Status == StatusOrdem.Confirmada || o.Status == StatusOrdem.Pendente) &&
                            o.LimiteDias.HasValue &&
                            o.Data.AddDays(o.LimiteDias.Value).Date < hoje)
                .ToListAsync();

            foreach (var ordem in ordensAtrasadas)
            {
                ordem.Status = StatusOrdem.Atrasada;
                ordem.ValorMulta = ordem.CalcularMulta(hoje);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<IActionResult> OnPostAtualizarAtrasosAsync()
        {
            var ordens = await _context.Ordens
                .Where(o => o.Tipo == TipoOrdem.Aluguel &&
                            (o.Status == StatusOrdem.Pendente || o.Status == StatusOrdem.Confirmada))
                .ToListAsync();

            var dataHoje = DateTime.Today;

            foreach (var ordem in ordens)
            {
                if (ordem.LimiteDias.HasValue)
                {
                    var dataEntrega = ordem.Data.AddDays(ordem.LimiteDias.Value);
                    if (dataHoje > dataEntrega)
                    {
                        ordem.Status = StatusOrdem.Atrasada;
                        ordem.ValorMulta = ordem.CalcularMulta(DateTime.Today);
                    }
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }
    }

    public static class OrdemExtensions
    {
        public static decimal CalcularMulta(this Ordem ordem, DateTime dataHoje)
        {
            if (ordem.Tipo != TipoOrdem.Aluguel || !ordem.LimiteDias.HasValue)
                return 0;

            var dataEntrega = ordem.Data.AddDays(ordem.LimiteDias.Value);

            if (dataHoje <= dataEntrega)
                return 0;

            var diasAtraso = (dataHoje - dataEntrega).Days;
            const decimal multaPorDia = 0.80m;

            return diasAtraso * multaPorDia;
        }
    }

}
