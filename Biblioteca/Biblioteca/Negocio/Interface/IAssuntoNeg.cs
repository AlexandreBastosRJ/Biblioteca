using Biblioteca.Negocio.Dto;

namespace Biblioteca.Negocio.Interface
{
    public interface IAssuntoNeg
    {
        List<AssuntoDto> ListarTodos();
        List<AssuntoDto> ListarCadastrados(int Id);
        void Adicionar(AssuntoDto assunto);        
        AssuntoDto ObterPorId(int id);
        void Atualizar(AssuntoDto assunto);
        void Excluir(int id);
        IEnumerable<AssuntoDto> ListarCadastrados(int? id);
    }
}