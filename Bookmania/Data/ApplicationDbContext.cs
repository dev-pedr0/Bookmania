using Bookmania.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bookmania.Data
{
    public class ApplicationDbContext : IdentityDbContext<Usuario>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Livro> Livros { get; set; }
        public DbSet<Tema> Temas { get; set; }
        public DbSet<Ordem> Ordens { get; set; }
        public DbSet<ItemOrdem> ItensOrdem { get; set; }
        public DbSet<ListaEspera> ListasDeEspera { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Livro>()
                .HasMany(l => l.Temas)
                .WithMany(t => t.Livros);

            modelBuilder.Entity<ItemOrdem>()
                .HasOne(i => i.Ordem)
                .WithMany(o => o.Itens)
                .HasForeignKey(i => i.OrdemId);

            modelBuilder.Entity<ListaEspera>()
                .HasOne(le => le.Livro)
                .WithMany(l => l.ListaEspera)
                .HasForeignKey(le => le.LivroId);


            modelBuilder.Entity<ListaEspera>()
                .HasOne(le => le.Usuario)
                .WithMany()
                .HasForeignKey(le => le.UsuarioId);

            modelBuilder.Entity<Ordem>()
            .Property(o => o.Status)
            .HasConversion<string>();

            modelBuilder.Entity<Ordem>()
                .Property(o => o.Tipo)
                .HasConversion<string>();
        }
    }
}
