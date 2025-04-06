using Biblioteca.Negocio.Dto; 

namespace Biblioteca.Negocio.Interface
{
    public interface ILivroNeg
    {
        IEnumerable<LivroDto> ObterTodos();
        LivroDto ObterPorId(int id);
        List<LivroDto> BuscarComFiltros(string? titulo, int? autorId, int? assuntoId);
        void SalvarLivroComRelacionamentos(LivroDto livroDto);
        void Atualizar(LivroDto livro);
        void Deletar(int id);
        List<RelatorioLivrosDto> RelatorioLivros();
    }
}
