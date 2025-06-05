using System;
using System.Security.Claims;
using Bookmania.Data;
using Bookmania.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookmania.Pages
{
    public class ConfirmacaoPedidoModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ConfirmacaoPedidoModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Usuario Usuario { get; set; }
        public Ordem OrdemCompra { get; set; }
        public Ordem OrdemAluguel { get; set; }

        public async Task<IActionResult> OnGetAsync(int? ordemCompraId, int? ordemAluguelId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Account/Login");
            }

            if (ordemCompraId.HasValue)
            {
                OrdemCompra = await _context.Ordens
                    .Include(o => o.Usuario)
                    .FirstOrDefaultAsync(o => o.Id == ordemCompraId && o.UsuarioId == userId);
            }

            if (ordemAluguelId.HasValue)
            {
                OrdemAluguel = await _context.Ordens
                    .Include(o => o.Usuario)
                    .FirstOrDefaultAsync(o => o.Id == ordemAluguelId && o.UsuarioId == userId);
            }

            if (OrdemCompra == null && OrdemAluguel == null)
                return NotFound();

            Usuario = OrdemCompra?.Usuario ?? OrdemAluguel?.Usuario;

            return Page();
        }
    }
}
