﻿namespace Bookmania.Models
{
    public class Tema
    {
        public int Id {  get; set; }
        public string Nome { get; set; }
        public List<Livro> Livros { get; set; }
    }
}
