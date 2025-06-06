using System.Security.Claims;
using Bookmania.Data;
using Bookmania.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookmania.Pages.Usuarios
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public class UsuarioViewModel
        {
            public Usuario Usuario { get; set; }
            public IList<string> Roles { get; set; }
        }

        public List<UsuarioViewModel> Usuarios { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Nome { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? CPF { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Role { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool MostrarInativos { get; set; } = false;

        public string RoleDoUsuarioLogado { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            RoleDoUsuarioLogado = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "";

            var query = _context.Users.AsQueryable();

            if (!MostrarInativos)
            {
                query = query.Where(u => u.Ativo);
            }

            if (!string.IsNullOrEmpty(Nome))
                query = query.Where(u => u.Nome.Contains(Nome));

            if (!string.IsNullOrEmpty(CPF))
                query = query.Where(u => u.CPF.Contains(CPF));

            var usuariosSemFiltro = await query.OrderBy(u => u.Nome).ToListAsync();

            var usuariosComRoles = new List<UsuarioViewModel>();
            foreach (var usuario in usuariosSemFiltro)
            {
                var roles = await _userManager.GetRolesAsync(usuario);

                if (!string.IsNullOrEmpty(Role) && !roles.Contains(Role))
                    continue;

                usuariosComRoles.Add(new UsuarioViewModel
                {
                    Usuario = usuario,
                    Roles = roles
                });
            }

            Usuarios = usuariosComRoles;
            return Page();
        }

        public bool PodeEditar(string roleUsuarioAlvo)
        {
            return RoleDoUsuarioLogado switch
            {
                "Funcionario" => roleUsuarioAlvo == "Cliente",
                "Gerente" => roleUsuarioAlvo == "Cliente" || roleUsuarioAlvo == "Funcionario",
                "Administrador" => true,
                _ => false
            };
        }
    }
}

