using Bookmania.Data;
using Bookmania.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookmania.Areas.Identity.Pages.Account.Manage
{
    public class OrdensListaModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public OrdensListaModel(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Ordem> Ordens { get; set; }

        [BindProperty(SupportsGet = true)]
        public TipoOrdem? FiltroTipo { get; set; }

        [BindProperty(SupportsGet = true)]
        public StatusOrdem? FiltroStatus { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Ordenacao { get; set; } = "desc";

        public async Task<IActionResult> OnGetAsync()
        {
            var usuario = await _userManager.GetUserAsync(User);

            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            var query = _context.Ordens
                .Include(o => o.Itens)
                    .ThenInclude(i => i.Livro)
                .Where(o => o.UsuarioId == usuario.Id)
                .AsQueryable();

            if (FiltroTipo != null)
            {
                query = query.Where(o => o.Tipo == FiltroTipo);
            }

            if (FiltroStatus != null)
            {
                query = query.Where(o => o.Status == FiltroStatus);
            }

            query = Ordenacao == "asc"
                ? query.OrderBy(o => o.Data)
                : query.OrderByDescending(o => o.Data);

            Ordens = await query.ToListAsync();

            return Page();
        }
    }
}
