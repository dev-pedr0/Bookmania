using Bookmania.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bookmania.Data
{
    public class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Usuario>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            context.Database.Migrate();

            string[] roles = { "Cliente", "Funcionario", "Gerente", "Administrador" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            if (!context.Users.Any())
            {
                var usuarios = new List<(string nome, string email, string role)>
                {
                    ("Cliente Teste", "cliente@bookmania.com", "Cliente"),
                    ("Funcionario Teste", "funcionario@bookmania.com", "Funcionario"),
                    ("Gerente Teste", "gerente@bookmania.com", "Gerente"),
                    ("Administrador Teste", "admin@bookmania.com", "Administrador")
                };
                foreach (var (nome, email, role) in usuarios)
                {
                    var user = new Usuario
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true,
                        Nome = nome,
                        CPF = "00000000000",
                        Idade = 30,
                        Telefone = "000000000",
                        Endereco = "Rua Exemplo, 123"
                    };

                    var result = await userManager.CreateAsync(user, "Senha@123"); // Senha padrão

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, role);
                    }
                }
            }

            if (!context.Livros.Any())
            {
                var temas = new List<Tema>
                {
                    new Tema { Nome = "Ficção" },
                    new Tema { Nome = "Romance" },
                    new Tema { Nome = "Terror" },
                    new Tema { Nome = "Autoajuda" },
                    new Tema { Nome = "Filosofia" },
                    new Tema { Nome = "Ciência" },
                    new Tema { Nome = "Fantasia" },
                };

                context.Temas.AddRange(temas);
                await context.SaveChangesAsync();

                var random = new Random();
                var livros = new List<Livro>();

                for (int i = 1; i <= 100; i++)
                {
                    var quantidade = random.Next(0, 20);
                    var precoCompra = Math.Round(random.NextDouble() * 300 + 20, 2);
                    var produzido = random.Next(0, 2) == 0;

                    var livro = new Livro
                    {
                        Titulo = $"Livro Exemplo {i}",
                        Autor = $"Autor {i}",
                        Editora = $"Editora {i}",
                        Paginas = random.Next(100, 1000),
                        Quantidade = quantidade,
                        PrecoCompra = (decimal)precoCompra,
                        PrecoAluguel = precoCompra > 200 ? 20m : 5m,
                        Produzido = produzido,
                        Temas = new List<Tema>()
                    }; ;

                    int temasCount = random.Next(1, 3);
                    var temasSelecionados = temas.OrderBy(x => random.Next()).Take(temasCount).ToList();
                    livro.Temas = temasSelecionados;

                    livro.VerificarCotacaoEspecial();

                    livros.Add(livro);
                }

                context.Livros.AddRange(livros);
                await context.SaveChangesAsync();
            }
        }
    }
}
