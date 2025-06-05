using System.Text.Json;
using Bookmania.Models;

namespace Bookmania.Helpers
{
    public class CarrinhoSessionHelper
    {
        private const string CarrinhoKey = "Carrinho";

        public static List<CarrinhoItem> GetCarrinho(ISession session)
        {
            var carrinhoJson = session.GetString(CarrinhoKey);
            if (string.IsNullOrEmpty(carrinhoJson))
            {
                return new List<CarrinhoItem>();
            }
            return JsonSerializer.Deserialize<List<CarrinhoItem>>(carrinhoJson) ?? new List<CarrinhoItem>();
        }

        public static void SaveCarrinho(ISession session, List<CarrinhoItem> carrinho)
        {
            session.SetString(CarrinhoKey, JsonSerializer.Serialize(carrinho));
        }

        public static void AdicionarItem(ISession session, Livro livro, int quantidade)
        {
            var carrinho = GetCarrinho(session);

            var itemExistente = carrinho.FirstOrDefault(i => i.LivroId == livro.Id);
            if (itemExistente != null)
            {
                itemExistente.Quantidade += quantidade;
            }
            else
            {
                carrinho.Add(new CarrinhoItem
                {
                    LivroId = livro.Id,
                    Titulo = livro.Titulo,
                    PrecoAluguel = livro.PrecoAluguel,
                    PrecoCompra = livro.PrecoCompra,
                    Quantidade = quantidade,
                    CotacaoEspecial = livro.LivroCotacaoEspecial
                });
            }
            SaveCarrinho(session, carrinho);
        }
    }
}
