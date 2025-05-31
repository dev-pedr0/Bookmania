using System.ComponentModel.DataAnnotations;

namespace Bookmania.Models
{
    public class Livro
    {
        public Livro()
        {
            ListaEspera = new List<ListaEspera>();
            Temas = new List<Tema>();
        }

        public int Id { get; set; }

        [Required]
        public string Titulo { get; set; }

        [Required]
        public string Autor { get; set; }

        public string Editora { get; set; }

        public int Paginas { get; set; }

        public int Quantidade { get; set; }

        public bool Produzido { get; set; } = true;

        public bool LivroCotacaoEspecial { get; set; } = false;

        public decimal PrecoCompra { get; set; }

        public decimal PrecoAluguel {get; set;}

        public bool Disponivel => Quantidade > 0;

        public ICollection<ListaEspera> ListaEspera { get; set; }

        public ICollection<Tema> Temas { get; set; }

        public void VerificarCotacaoEspecial()
        {
            if (PrecoCompra > 200 || !Produzido)
            {
                LivroCotacaoEspecial = true;
            }
            else
            {
                LivroCotacaoEspecial = false;
            }
        }

        public decimal CalcularValorAluguel(int diasAluguel)
        {
            const int periodoBase = 15; // base de preço em dias
            int periodos = (int)Math.Ceiling(diasAluguel / (double)periodoBase);
            return PrecoAluguel * periodos;
        }

        public decimal CalcularMulta(int diasAtraso)
        {
            const decimal valorMultaPorDia = 0.80m;
            return valorMultaPorDia * diasAtraso;
        }
    }
}
