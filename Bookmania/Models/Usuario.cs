using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Bookmania.Models
{
    public class Usuario : IdentityUser
    {
        [Required]
        public String Nome { get; set; }

        [Required]
        public int Idade { get; set; }

        [Required]
        public string CPF { get; set;}

        public string Endereco { get; set;}

        public string Telefone { get; set;}
    }
}
