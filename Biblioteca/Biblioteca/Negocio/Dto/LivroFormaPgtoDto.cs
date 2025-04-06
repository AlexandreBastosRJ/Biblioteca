namespace Biblioteca.Negocio.Dto
{
    public class LivroFormaPgtoDto
    {
        public int Livro_Codl { get; set; }
        public int FormaVenda_CodFv { get; set; }
        public string? DescricaoFormaPgto { get; set; } = string.Empty;
        public decimal? Valor_Livro_FormaVenda { get; set; }
    }
}
