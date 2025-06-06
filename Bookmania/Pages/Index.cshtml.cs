using System.Security.Claims;
using Bookmania.Data;
using Bookmania.Helpers;
using Bookmania.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Bookmania.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        private readonly ApplicationDbContext _context;

        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public PaginatedList<Livro> Livros { get; set; }

        public string CurrentSort { get; set; }
        public string TitleSort { get; set; }
        public string AuthorSort { get; set; }
        public string TemaSort { get; set; }
        public string PrecoSort { get; set; }
        public string QuantidadeSort { get; set; }

        public string CurrentTitle { get; set; }
        public string CurrentAuthor { get; set; }
        public string CurrentTema { get; set; }
        public bool SomenteDisponiveis { get; set; }
        public bool MostrarCotacaoEspecial { get; set; }
        public SelectList TemasSelectList { get; set; }

        public async Task OnGetAsync(string sortOrder, string currentTitle, string currentAuthor, string currentTema,
                                 bool somenteDisponiveis, bool mostrarCotacaoEspecial, int pageIndex = 1)
        {
            CurrentSort = sortOrder;
            CurrentTitle = currentTitle;
            CurrentAuthor = currentAuthor;
            CurrentTema = currentTema;
            SomenteDisponiveis = somenteDisponiveis;
            MostrarCotacaoEspecial = mostrarCotacaoEspecial;

            TemasSelectList = new SelectList(await _context.Temas.OrderBy(t => t.Nome).ToListAsync(), "Nome", "Nome");

            TitleSort = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            AuthorSort = sortOrder == "Author" ? "author_desc" : "Author";
            TemaSort = sortOrder == "Tema" ? "tema_desc" : "Tema";
            PrecoSort = sortOrder == "Preco" ? "preco_desc" : "Preco";
            QuantidadeSort = sortOrder == "Quantidade" ? "quantidade_desc" : "Quantidade";

            var livros = _context.Livros.Include(l => l.Temas).AsQueryable();

            if (!string.IsNullOrEmpty(CurrentTitle))
            {
                livros = livros.Where(l => l.Titulo.Contains(CurrentTitle));
            }

            if (!string.IsNullOrEmpty(CurrentAuthor))
            {
                livros = livros.Where(l => l.Autor.Contains(CurrentAuthor));
            }

            if (!string.IsNullOrEmpty(CurrentTema))
            {
                livros = livros.Where(l => l.Temas.Any(t => t.Nome == CurrentTema));
            }

            if (SomenteDisponiveis)
            {
                livros = livros.Where(l => l.Quantidade > 0);
            }

            if (!MostrarCotacaoEspecial)
            {
                livros = livros.Where(l => !l.LivroCotacaoEspecial);
            }

            livros = sortOrder switch
            {
                "title_desc" => livros.OrderByDescending(s => s.Titulo),
                "Author" => livros.OrderBy(s => s.Autor),
                "author_desc" => livros.OrderByDescending(s => s.Autor),
                "Tema" => livros.OrderBy(s => s.Temas.FirstOrDefault().Nome),
                "tema_desc" => livros.OrderByDescending(s => s.Temas.FirstOrDefault().Nome),
                "Preco" => livros.OrderBy(s => s.PrecoCompra),
                "preco_desc" => livros.OrderByDescending(s => s.PrecoCompra),
                "Quantidade" => livros.OrderBy(s => s.Quantidade),
                "quantidade_desc" => livros.OrderByDescending(s => s.Quantidade),
                _ => livros.OrderBy(s => s.Titulo),
            };

            int pageSize = 10;
            Livros = await PaginatedList<Livro>.CreateAsync(livros.AsNoTracking(), pageIndex, pageSize);
        }

        public IActionResult OnPostAdicionarAoCarrinho(int livroId, int quantidade)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage();
            }

            var livro = _context.Livros.Include(l => l.Temas).FirstOrDefault(l => l.Id == livroId);
            if (livro == null || quantidade <= 0)
            {
                return RedirectToPage();
            }
            CarrinhoSessionHelper.AdicionarItem(HttpContext.Session, livro, quantidade);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEntrarListaEsperaAsync(int livroId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var livro = await _context.Livros.Include(l => l.ListaEspera)
                                             .FirstOrDefaultAsync(l => l.Id == livroId);

            if (livro == null)
            {
                ModelState.AddModelError("", "Livro não encontrado.");
                return Page();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var jaNaLista = livro.ListaEspera.Any(le => le.UsuarioId == userId);

            if (!jaNaLista)
            {
                var listaEspera = new ListaEspera
                {
                    LivroId = livroId,
                    UsuarioId = userId,
                    DataSolicitacao = DateTime.Now
                };

                _context.ListaEspera.Add(listaEspera);
                await _context.SaveChangesAsync();
            }

            TempData["Mensagem"] = $"Você entrou na lista de espera do livro '{livro.Titulo}' com sucesso!";

            return RedirectToPage();
        }
    }
}
