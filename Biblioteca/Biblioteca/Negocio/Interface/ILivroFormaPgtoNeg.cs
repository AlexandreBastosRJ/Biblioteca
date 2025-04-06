using Biblioteca.Negocio.Dto;

namespace Biblioteca.Negocio.Interface
{

    public interface ILivroFormaPgtoNeg
    {
        List<LivroFormaPgtoDto> ObterPorId(int livroId);
        void SalvarLivroFormaPgto(LivroFormaPgtoDto livroFormaPgtoDto);
        void Atualizar(LivroFormaPgtoDto formaPgto);
        void Deletar(int id);
        string ValidaExiteFormaPgtonoLivro(int id);

    }
}
