using Bookmania.Data;
using Bookmania.Helpers;
using Bookmania.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookmania.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;
        private readonly ApplicationDbContext _context;

        public PrivacyModel(ILogger<PrivacyModel> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public List<CarrinhoItem> ItensCarrinho { get; set; } = new();

        public IActionResult OnGet()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Page();
            }

            var itensSessao = CarrinhoSessionHelper.GetCarrinho(HttpContext.Session);

            ItensCarrinho = itensSessao.Select(item =>
            {
                var livro = _context.Livros.FirstOrDefault(l => l.Id == item.LivroId);

                return new CarrinhoItem
                {
                    LivroId = item.LivroId,
                    Titulo = item.Titulo,
                    PrecoCompra = item.PrecoCompra,
                    PrecoAluguel = item.PrecoAluguel,
                    Quantidade = item.Quantidade,
                    CotacaoEspecial = livro.LivroCotacaoEspecial
                };
            }).ToList();

            return Page();
        }
    }

}
