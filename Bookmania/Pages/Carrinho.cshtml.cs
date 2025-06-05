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

        [BindProperty]
        public List<CarrinhoItem> ItensCarrinho { get; set; } = new();

        public IActionResult OnGet()
        {
            _logger.LogInformation("Teste de log no OnGet do Carrinho.");

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
                    CotacaoEspecial = livro.LivroCotacaoEspecial,
                    QuantidadeMax = livro.Quantidade,
                    ModoAluguel = item.ModoAluguel,
                    PeriodoAluguelSemanas = item.PeriodoAluguelSemanas
                };
            }).ToList();

            return Page();
        }

        public IActionResult OnPost()
        {
            var carrinho = CarrinhoSessionHelper.GetCarrinho(HttpContext.Session);

            foreach (var itemForm in ItensCarrinho)
            {
                _logger.LogInformation("ResumoPedido - Livro: {Titulo}, ModoAluguel: {ModoAluguel}, Periodo: {Periodo}, Qtde: {Quantidade}",
                     itemForm.Titulo, itemForm.ModoAluguel, itemForm.PeriodoAluguelSemanas, itemForm.Quantidade);

                var itemCarrinho = carrinho.FirstOrDefault(c => c.LivroId == itemForm.LivroId);
                if (itemCarrinho != null)
                {
                    _logger.LogInformation("ResumoPedido2 - Livro: {Titulo}, ModoAluguel: {ModoAluguel}, Periodo: {Periodo}, Qtde: {Quantidade}",
                    itemCarrinho.Titulo, itemCarrinho.ModoAluguel, itemCarrinho.PeriodoAluguelSemanas, itemCarrinho.Quantidade);

                    itemCarrinho.Quantidade = itemForm.Quantidade;
                    itemCarrinho.ModoAluguel = itemForm.ModoAluguel;
                    itemCarrinho.PeriodoAluguelSemanas = itemForm.PeriodoAluguelSemanas < 1 ? 1 : itemForm.PeriodoAluguelSemanas;
                    _logger.LogInformation("ResumoPedido2 - Livro: {Titulo}, ModoAluguel: {ModoAluguel}, Periodo: {Periodo}, Qtde: {Quantidade}",
                    itemCarrinho.Titulo, itemCarrinho.ModoAluguel, itemCarrinho.PeriodoAluguelSemanas, itemCarrinho.Quantidade);
                }
            }

            CarrinhoSessionHelper.SaveCarrinho(HttpContext.Session, carrinho);

            return RedirectToPage("/ResumoPedido");
        }

        public IActionResult OnGetRemoverDoCarrinho(int livroId)
        {
            var carrinho = CarrinhoSessionHelper.GetCarrinho(HttpContext.Session);
            var item = carrinho.FirstOrDefault(i => i.LivroId == livroId);

            if (item != null)
            {
                carrinho.Remove(item);
                CarrinhoSessionHelper.SaveCarrinho(HttpContext.Session, carrinho);
            }

            return RedirectToPage();
        }
    }

}
