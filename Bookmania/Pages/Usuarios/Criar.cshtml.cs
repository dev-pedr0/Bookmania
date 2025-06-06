using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Bookmania.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookmania.Pages.Usuarios
{
    [Authorize(Roles = "Funcionario,Gerente,Administrador")]
    public class CriarModel : PageModel
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CriarModel(UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public Usuario NovoUsuario { get; set; } = new();

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [BindProperty]
        [Required]
        public string RoleSelecionada { get; set; }

        public List<string> RolesDisponiveis { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            await CarregarRolesDisponiveisAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await CarregarRolesDisponiveisAsync();

            if (!ModelState.IsValid)
            {
                var erros = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine("Erros: " + string.Join(", ", erros));
                return Page();
            }

            var usuario = new Usuario
            {
                UserName = NovoUsuario.Email,
                Email = NovoUsuario.Email,
                Nome = NovoUsuario.Nome,
                CPF = NovoUsuario.CPF,
                Idade = NovoUsuario.Idade,
                Telefone = NovoUsuario.Telefone,
                Endereco = NovoUsuario.Endereco,
                Ativo = true
            };

            var resultado = await _userManager.CreateAsync(usuario, Senha);

            if (!resultado.Succeeded)
            {
                foreach (var erro in resultado.Errors)
                    ModelState.AddModelError(string.Empty, erro.Description);

                return Page();
            }

            await _userManager.AddToRoleAsync(usuario, RoleSelecionada);

            return RedirectToPage("/Usuarios/Index");
        }

        private async Task CarregarRolesDisponiveisAsync()
        {
            var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "";

            if (userRole == "Funcionario")
                RolesDisponiveis = new() { "Cliente" };
            else if (userRole == "Gerente")
                RolesDisponiveis = new() { "Cliente", "Funcionario" };
            else if (userRole == "Administrador")
                RolesDisponiveis = _roleManager.Roles.Select(r => r.Name).ToList();
        }
    }
}
