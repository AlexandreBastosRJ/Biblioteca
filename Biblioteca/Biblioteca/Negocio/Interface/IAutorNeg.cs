using Biblioteca.Negocio.Dto;


namespace Biblioteca.Negocio.Interface
{
    public interface IAutorNeg
    {
        List<AutorDto> ObterTodos();
        List<AutorDto> ListarCadastrados(int id);
        void Adicionar(AutorDto autor);
        AutorDto ObterPorId(int id);       
        void Atualizar(AutorDto autor);
        void Excluir(int id);
    }
}
