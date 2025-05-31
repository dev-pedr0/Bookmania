namespace Bookmania.Models
{
    public class ItemOrdem
    {
        public int Id { get; set; }
        public int OrdemId { get; set; }
        public Ordem Ordem { get; set; }
        public int LivroId { get; set; }
        public Livro Livro { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
    }
}
