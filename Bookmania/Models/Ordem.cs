﻿namespace Bookmania.Models
{

    public enum TipoOrdem
    {
        Compra,
        Aluguel
    }

    public enum StatusOrdem
    {
        Pendente,
        Confirmada,
        Atrasada,
        Cancelada,
        Concluida
    }

    public class Ordem
    {
        public int Id { get; set; }
        public string UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public DateTime Data { get; set; } = DateTime.Now;
        public decimal ValorTotal { get; set; }
        public TipoOrdem Tipo {  get; set; }
        public StatusOrdem Status { get; set; } = StatusOrdem.Pendente;

        public int? LimiteDias { get; set; }
        public decimal? ValorMulta { get; set; }

        public List<ItemOrdem> Itens { get; set; }
    }
}
