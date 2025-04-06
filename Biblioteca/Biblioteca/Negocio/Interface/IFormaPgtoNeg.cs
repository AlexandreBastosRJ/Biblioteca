using Biblioteca.Negocio.Dto;

namespace Biblioteca.Negocio.Interface
{
    public interface IFormaPgtoNeg
    {
        FormaPgtoDto ObterPorId(int id);
        List<FormaPgtoDto> ListarTodos();
        void Adicionar(FormaPgtoDto dto);
        void Atualizar(FormaPgtoDto dto);
        void Excluir(int id);
    }
}
