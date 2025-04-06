using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Biblioteca.Negocio.Dto;
using Biblioteca.Negocio.Implementacao;
using Biblioteca.Negocio.Interface;
using Microsoft.Data.SqlClient;

namespace Biblioteca.Pages
{
    public class CadastroAssuntosModel : PageModel
    {
        [BindProperty]
        public required AssuntoDto Assunto { get; set; }
        private readonly IAssuntoNeg _assuntoNeg;
        public List<AssuntoDto> ListaAssuntos { get; set; } = new();

      

        public void OnGet(int? id)
        {
            if (id.HasValue)
            {
                Assunto = _assuntoNeg.ObterPorId(id.Value);
            }
            else
            {
                Assunto = new AssuntoDto();
            }

            ListaAssuntos = _assuntoNeg.ListarTodos();
        }

        public IActionResult OnPostEditar()
        {
            if (!ModelState.IsValid)
                return Page();
            try
            {
                _assuntoNeg.Atualizar(Assunto);
                TempData["Mensagem"] = $"Assunto atualizado com sucesso!";
            }
            catch (SqlException ex)
            {
                TempData["Erro"] = $"Erro ao atualizar: {ex.Message}";
            }

            ListaAssuntos = _assuntoNeg.ListarTodos();
            return Page();
        }

        public IActionResult OnPostExcluir(int AssuntoId)
        {
            try
            {
                _assuntoNeg.Excluir(AssuntoId);
                TempData["Mensagem"] = "Assunto excluído com sucesso!";
            }
            catch (SqlException ex)
            {
                TempData["Erro"] = "Erro ao excluir o assunto. Verifique se ele está sendo usado em outro lugar.";
            }

            ListaAssuntos = _assuntoNeg.ListarTodos();
            return Page();
        }

        public CadastroAssuntosModel(IAssuntoNeg assuntoNeg)
        {
            _assuntoNeg = assuntoNeg;
        }


        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                var novoAssunto = new AssuntoDto { Descricao = Assunto.Descricao };
                _assuntoNeg.Adicionar(novoAssunto);
                ListaAssuntos = _assuntoNeg.ListarTodos();


                TempData["Mensagem"] = $"Assunto '{Assunto.Descricao}' cadastrado com sucesso!";
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
    }
}
