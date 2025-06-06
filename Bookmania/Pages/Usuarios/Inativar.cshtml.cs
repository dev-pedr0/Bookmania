using Bookmania.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookmania.Pages.Usuarios
{
    [Authorize(Roles = "Funcionario,Gerente,Administrador")]
    public class InativarModel : PageModel
    {
        private readonly UserManager<Usuario> _userManager;

        public InativarModel(UserManager<Usuario> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public Usuario Usuario { get; set; }

        public string RoleUsuario { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            Usuario = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id && u.Ativo);
            if (Usuario == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(Usuario);
            RoleUsuario = roles.FirstOrDefault();

            if (!PodeInativar(RoleUsuario))
                return Forbid();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var usuario = await _userManager.FindByIdAsync(Usuario.Id);

            if (usuario == null || !usuario.Ativo)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(usuario);
            var role = roles.FirstOrDefault();

            if (!PodeInativar(role))
                return Forbid();

            usuario.Ativo = false;
            var resultado = await _userManager.UpdateAsync(usuario);

            if (!resultado.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Erro ao inativar o usuário.");
                return Page();
            }

            return RedirectToPage("/Usuarios/Index");
        }

        private bool PodeInativar(string role)
        {
            if (User.IsInRole("Administrador"))
                return true;

            if (User.IsInRole("Gerente") && (role == "Funcionario" || role == "Cliente"))
                return true;

            if (User.IsInRole("Funcionario") && role == "Cliente")
                return true;

            return false;
        }
    }
}
