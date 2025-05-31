namespace Bookmania.Models
{
    public class ListaEspera
    {
        public int Id { get; set; }
        public string UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public int LivroId { get; set; }
        public Livro Livro { get; set; }
        public DateTime DataSolicitacao { get; set; } = DateTime.Now;
    }
}
