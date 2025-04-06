using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Biblioteca.Negocio.Dto;
using Biblioteca.Negocio.Implementacao;
using Biblioteca.Negocio.Interface;
using Microsoft.Data.SqlClient;

namespace Biblioteca.Pages
{
    public class CadastroAutoresModel : PageModel
    {
        [BindProperty]    
        public required AutorDto Autor { get; set; }
        private readonly IAutorNeg _autorNeg;
        public List<AutorDto> ListaAutores { get; set; } = new();

        public CadastroAutoresModel(IAutorNeg autorNeg)
        {
            _autorNeg = autorNeg;
        }


        public void OnGet()
        {
            Autor = new AutorDto();
            ListaAutores = _autorNeg.ObterTodos();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                var novoAutor = new AutorDto { Nome = Autor.Nome};
                _autorNeg.Adicionar(novoAutor);
                ListaAutores = _autorNeg.ObterTodos();


                TempData["Mensagem"] = $"Autor '{Autor.Nome}' cadastrado com sucesso!";
            }
            catch (SqlException sqlEx)
            {

                TempData["Erro"] = $"Erro ao acessar o banco de dados: {sqlEx.Message}";

            }
            catch (InvalidOperationException invOpEx)
            {
                TempData["Erro"] = $"Erro na operação: {invOpEx.Message}";
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Ocorreu um erro inesperado. Tente novamente.";
            }
            return Page();
        }

        public IActionResult OnPostEditar(int AutorId, string Nome)
        {
            if (!ModelState.IsValid)
                return Page();

            try
            {
                _autorNeg.Atualizar(Autor);
                TempData["Mensagem"] = $"Autor atualizado com sucesso!";
            }
            catch (SqlException ex)
            {
                TempData["Erro"] = $"Erro ao atualizar: {ex.Message}";
            }

            ListaAutores = _autorNeg.ObterTodos();
            return Page();

          
        }

        public IActionResult OnPostExcluir(int AutorId)
        {

            try
            {
                _autorNeg.Excluir(AutorId);
                TempData["Mensagem"] = "Autor excluído com sucesso!";
            }
            catch (SqlException ex)
            {
                TempData["Erro"] = "Erro ao excluir o Autor. Verifique se ele está sendo usado em outro lugar.";
            }

            ListaAutores = _autorNeg.ObterTodos();
            return Page();


        }

       
    }

}
