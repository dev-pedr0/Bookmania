using Bookmania.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Bookmania.Pages.Usuarios
{
    [Authorize(Roles = "Funcionario,Gerente,Administrador")]
    public class EditarModel : PageModel
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public EditarModel(UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public Usuario Usuario { get; set; }

        [BindProperty]
        public string RoleSelecionada { get; set; }

        public SelectList RolesDisponiveis { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null) return NotFound();

            Usuario = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id && u.Ativo);
            if (Usuario == null) return NotFound();

            var rolesUsuario = await _userManager.GetRolesAsync(Usuario);
            RoleSelecionada = rolesUsuario.FirstOrDefault();

            RolesDisponiveis = await ObterRolesPermitidasAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                RolesDisponiveis = await ObterRolesPermitidasAsync();
                return Page();
            }

            var usuarioBanco = await _userManager.FindByIdAsync(Usuario.Id);
            if (usuarioBanco == null || !usuarioBanco.Ativo)
                return NotFound();

            if (!await PodeEditarRoleAsync(usuarioBanco.Id)) return Forbid();

            usuarioBanco.Nome = Usuario.Nome;
            usuarioBanco.Idade = Usuario.Idade;
            usuarioBanco.CPF = Usuario.CPF;
            usuarioBanco.Endereco = Usuario.Endereco;
            usuarioBanco.Telefone = Usuario.Telefone;
            usuarioBanco.Email = Usuario.Email;
            usuarioBanco.UserName = Usuario.Email;

            var resultadoUpdate = await _userManager.UpdateAsync(usuarioBanco);
            if (!resultadoUpdate.Succeeded)
            {
                foreach (var erro in resultadoUpdate.Errors)
                    ModelState.AddModelError(string.Empty, erro.Description);

                RolesDisponiveis = await ObterRolesPermitidasAsync();
                return Page();
            }

            var rolesAtuais = await _userManager.GetRolesAsync(usuarioBanco);
            await _userManager.RemoveFromRolesAsync(usuarioBanco, rolesAtuais);
            await _userManager.AddToRoleAsync(usuarioBanco, RoleSelecionada);

            return RedirectToPage("/Usuarios/Index");
        }

        private async Task<SelectList> ObterRolesPermitidasAsync()
        {
            var rolesPermitidas = new List<string>();

            if (User.IsInRole("Administrador"))
                rolesPermitidas = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            else if (User.IsInRole("Gerente"))
                rolesPermitidas = new List<string> { "Cliente", "Funcionario" };
            else if (User.IsInRole("Funcionario"))
                rolesPermitidas = new List<string> { "Cliente" };

            return new SelectList(rolesPermitidas);
        }

        private async Task<bool> PodeEditarRoleAsync(string idUsuario)
        {
            var roles = await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(idUsuario));
            var role = roles.FirstOrDefault();

            if (User.IsInRole("Administrador")) return true;
            if (User.IsInRole("Gerente") && (role == "Funcionario" || role == "Cliente")) return true;
            if (User.IsInRole("Funcionario") && role == "Cliente") return true;

            return false;
        }
    }
}
