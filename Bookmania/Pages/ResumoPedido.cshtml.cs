using System.Runtime.Intrinsics.Arm;
using Bookmania.Data;
using Bookmania.Helpers;
using Bookmania.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bookmania.Pages
{
    public class ResumoPedidoModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;
        private readonly ILogger<ResumoPedidoModel> _logger;

        public ResumoPedidoModel(ApplicationDbContext context, UserManager<Usuario> userManager, ILogger<ResumoPedidoModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public List<CarrinhoItem> ItensCompra { get; set; } = new();
        public List<CarrinhoItem> ItensAluguel { get; set; } = new();

        public decimal TotalCompra { get; set; }
        public decimal TotalAluguel { get; set; }
        public decimal TotalGeral => TotalCompra + TotalAluguel;

        public Usuario Usuario { get; set; }

        public IActionResult OnGet()
        {
            var carrinho = CarrinhoSessionHelper.GetCarrinho(HttpContext.Session);

            if (carrinho == null || !carrinho.Any())
            {
                return RedirectToPage("/Carrinho");
            }

            SepararItens(carrinho);
            CarregarDadosUsuario();

            return Page();
        }

        public IActionResult OnPost()
        {
            var carrinho = CarrinhoSessionHelper.GetCarrinho(HttpContext.Session);

            if (carrinho == null || !carrinho.Any())
            {
                return RedirectToPage("/Carrinho");
            }

            var livrosIds = carrinho.Select(i => i.LivroId).ToList();
            var livrosDb = _context.Livros.Where(l => livrosIds.Contains(l.Id)).ToList();

            decimal totalCompra = 0;
            decimal totalAluguel = 0;

            foreach (var item in carrinho)
            {
                var livro = livrosDb.FirstOrDefault(l => l.Id == item.LivroId);
                if (livro == null)
                {
                    ModelState.AddModelError("", $"Livro {item.Titulo} não encontrado.");
                    return Page();
                }

                if (item.Quantidade > livro.Quantidade)
                {
                    ModelState.AddModelError("", $"Quantidade indisponível para {livro.Titulo}.");
                    return Page();
                }

                item.PrecoCompra = livro.PrecoCompra;
                item.PrecoAluguel = livro.PrecoAluguel;

                if (item.ModoAluguel)
                {
                    int dias = item.PeriodoAluguelSemanas * 7;
                    decimal valorAluguel = livro.CalcularValorAluguel(dias);
                    totalAluguel += valorAluguel * item.Quantidade;
                }
                else
                {
                    totalCompra += item.PrecoCompra * item.Quantidade;
                }
            }

            CarregarDadosUsuario();

            Ordem ordemCompra = null;
            Ordem ordemAluguel = null;

            if (carrinho.Any(i => !i.ModoAluguel))
            {
                ordemCompra = CriarOrdem(TipoOrdem.Compra,
                    carrinho.Where(i => !i.ModoAluguel).ToList(),
                    totalCompra, null);

                _context.Ordens.Add(ordemCompra);
            }

            if(carrinho.Any(i => i.ModoAluguel))
            {
                int limiteDias = carrinho
                    .Where(i => i.ModoAluguel)
                    .Max(i => i.PeriodoAluguelSemanas) * 7;

                ordemAluguel = CriarOrdem(TipoOrdem.Aluguel,
                    carrinho.Where(i => i.ModoAluguel).ToList(),
                    totalAluguel,
                    limiteDias);

                _context.Ordens.Add(ordemAluguel);
            }

            _context.SaveChanges();

            var parametros = new Dictionary<string, object>();

            if (ordemCompra != null)
                parametros["ordemCompraId"] = ordemCompra.Id;

            if (ordemAluguel != null)
                parametros["ordemAluguelId"] = ordemAluguel.Id;

            CarrinhoSessionHelper.SaveCarrinho(HttpContext.Session, new List<CarrinhoItem>());

            _logger.LogInformation("Ordem Criada!");

            return RedirectToPage("/ConfirmacaoPedido", parametros);
        }

        private void SepararItens(List<CarrinhoItem> carrinho)
        {
            ItensCompra = carrinho.Where(i => !i.ModoAluguel).ToList();
            ItensAluguel = carrinho.Where(i => i.ModoAluguel).ToList();

            TotalCompra = ItensCompra.Sum(i => i.PrecoCompra * i.Quantidade);

            TotalAluguel = ItensAluguel.Sum(i =>
            {
                var livro = _context.Livros.FirstOrDefault(l => l.Id == i.LivroId);
                if (livro == null) return 0;

                int dias = i.PeriodoAluguelSemanas * 7;
                decimal valor = livro.CalcularValorAluguel(dias);
                return valor * i.Quantidade;
            });
        }

        private void CarregarDadosUsuario()
        {
            var userId = _userManager.GetUserId(User);
            Usuario = _context.Users.FirstOrDefault(u => u.Id == userId);
        }

        private Ordem CriarOrdem(TipoOrdem tipo, List<CarrinhoItem> itens, decimal total, int? limiteDias)
        {
            var userId = _userManager.GetUserId(User);

            var ordem = new Ordem
            {
                UsuarioId = userId,
                Data = DateTime.Now,
                Tipo = tipo,
                ValorTotal = total,
                LimiteDias = tipo == TipoOrdem.Aluguel ? itens.Max(i => i.PeriodoAluguelSemanas * 7) : null,
                ValorMulta = tipo == TipoOrdem.Aluguel ? 10 : null, // Exemplo
                Itens = itens.Select(i => new ItemOrdem
                {
                    LivroId = i.LivroId,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = tipo == TipoOrdem.Compra
                        ? i.PrecoCompra
                        : _context.Livros.First(l => l.Id == i.LivroId).CalcularValorAluguel(i.PeriodoAluguelSemanas * 7)
                }).ToList(),
                Status = StatusOrdem.Pendente
            };

            return ordem;
        }
    }
}
