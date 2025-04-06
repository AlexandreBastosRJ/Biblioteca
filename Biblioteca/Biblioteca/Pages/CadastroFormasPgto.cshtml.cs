using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Biblioteca.Negocio.Dto;
using Biblioteca.Negocio.Interface;
using Microsoft.Data.SqlClient;

namespace Biblioteca.Pages
{
    public class CadastroFormaPgtoModel : PageModel
    {
        [BindProperty]
        public required FormaPgtoDto FormaPgto { get; set; }

        private readonly IFormaPgtoNeg _formaPgtoNeg;

        public List<FormaPgtoDto> ListaFormasPgto { get; set; } = new();

        public CadastroFormaPgtoModel(IFormaPgtoNeg formaPgtoNeg)
        {
            _formaPgtoNeg = formaPgtoNeg;
        }

        public void OnGet(int? id)
        {
            if (id.HasValue)
            {
                FormaPgto = _formaPgtoNeg.ObterPorId(id.Value);
            }
            else
            {
                FormaPgto = new FormaPgtoDto();
            }

            ListaFormasPgto = _formaPgtoNeg.ListarTodos();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            try
            {
                _formaPgtoNeg.Adicionar(FormaPgto);
                TempData["Mensagem"] = $"Forma de pagamento '{FormaPgto.Descricao}' cadastrada com sucesso!";
            }
            catch (SqlException ex)
            {
                TempData["Erro"] = $"Erro ao acessar o banco de dados: {ex.Message}";
            }

            ListaFormasPgto = _formaPgtoNeg.ListarTodos();
            return Page();
        }

        public IActionResult OnPostEditar()
        {
            if (!ModelState.IsValid)
                return Page();

            try
            {
                _formaPgtoNeg.Atualizar(FormaPgto);
                TempData["Mensagem"] = "Forma de pagamento atualizada com sucesso!";
            }
            catch (SqlException ex)
            {
                TempData["Erro"] = $"Erro ao atualizar: {ex.Message}";
            }

            ListaFormasPgto = _formaPgtoNeg.ListarTodos();
            return Page();
        }

        public IActionResult OnPostExcluir(int CodFv)
        {
            try
            {
                _formaPgtoNeg.Excluir(CodFv);
                TempData["Mensagem"] = "Forma de pagamento excluída com sucesso!";
            }
            catch (SqlException ex)
            {
                TempData["Erro"] = $"Erro ao excluir: {ex.Message}";
            }

            ListaFormasPgto = _formaPgtoNeg.ListarTodos();
            return Page();
        }
    }
}
