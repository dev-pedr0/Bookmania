using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Bookmania.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class EditAdditionalDataModel : PageModel
{
    private readonly UserManager<Usuario> _userManager;

    public EditAdditionalDataModel(UserManager<Usuario> userManager)
    {
        _userManager = userManager;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        [Required]
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        [Required]
        [Range(0, 120, ErrorMessage = "Idade inválida.")]
        [Display(Name = "Idade")]
        public int Idade { get; set; }

        [Required]
        [Display(Name = "CPF")]
        public string CPF { get; set; }

        [Display(Name = "Endereço")]
        public string Endereco { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return NotFound("Usuário não encontrado.");

        Input = new InputModel
        {
            Nome = user.Nome,
            Idade = user.Idade,
            CPF = user.CPF,
            Endereco = user.Endereco
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return NotFound("Usuário não encontrado.");

        user.Nome = Input.Nome;
        user.Idade = Input.Idade;
        user.CPF = Input.CPF;
        user.Endereco = Input.Endereco;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return Page();
        }

        TempData["StatusMessage"] = "Dados atualizados com sucesso.";
        return RedirectToPage();
    }
}
