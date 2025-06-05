namespace Bookmania.Models
{
    public class CarrinhoItem
    {
        public int LivroId { get; set; }
        public string Titulo { get; set; }
        public decimal PrecoAluguel { get; set; }
        public decimal PrecoCompra { get; set; }
        public int Quantidade { get; set; }
        public bool CotacaoEspecial { get; set; }
        public int QuantidadeMax { get; set; }
        public bool ModoAluguel { get; set; } = false;
        public int PeriodoAluguelSemanas { get; set; } = 1;
    }
}
