using Biblioteca.Negocio.Dto;
using Biblioteca.Negocio.Implementacao;
using Biblioteca.Negocio.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Identity.Client;
using System.Collections.Generic;

namespace Biblioteca.Pages
{
    public class CadastroLivroModel : PageModel
    {
       
        private readonly IAutorNeg _autorNeg;
        private readonly IAssuntoNeg _assuntoNeg;
        private readonly ILivroNeg _livroNeg;
        private readonly ILivroFormaPgtoNeg _livroPgtoNeg;
        private readonly IFormaPgtoNeg _formaPgtoNeg;



        public CadastroLivroModel(IAutorNeg autorNeg, IAssuntoNeg assuntoNeg, ILivroNeg livroNeg, ILivroFormaPgtoNeg livroPgtoNeg, IFormaPgtoNeg formaPgtoNeg)
        {
            _autorNeg = autorNeg;
            _assuntoNeg = assuntoNeg;
            _livroNeg = livroNeg;
            _livroPgtoNeg = livroPgtoNeg;
            _formaPgtoNeg = formaPgtoNeg;

            Autores = new List<SelectListItem>();
            Assuntos = new List<SelectListItem>();
            AutoresCadastrados = new List<SelectListItem>();
            AssuntosCadastrados = new List<SelectListItem>();
            FormasPgtoCadastradas = new List<FormaPgtoDto>();


            Livro = new LivroDto();
           
        }

        [BindProperty]
        public required LivroDto Livro { get; set; }

        public required List<SelectListItem> Autores { get; set; }
        public required List<SelectListItem> Assuntos { get; set; }
        public required List<SelectListItem> AutoresCadastrados { get; set; }
        public required List<SelectListItem> AssuntosCadastrados { get; set; }
     

       
        public List<LivroFormaPgtoDto> ListaLivroFormaPgto { get; set; } = new();
        public required AutorDto Autor { get; set; }
        public required AssuntoDto Assunto { get; set; }
        public int LivroId { get; set; }
        public List<FormaPgtoDto> FormasPgtoCadastradas { get; set; }

        public void OnGet(int? id)
        {

            var listaAutores = _autorNeg.ObterTodos();
            Autores = listaAutores.Select(a => new SelectListItem
            {
                Value = a.AutorId.ToString(),
                Text = a.Nome
            }).ToList();

          
            var listaAssuntos = _assuntoNeg.ListarTodos();
            Assuntos = listaAssuntos.Select(a => new SelectListItem
            {
                Value = a.AssuntoId.ToString(),
                Text = a.Descricao
            }).ToList();

            FormasPgtoCadastradas = _formaPgtoNeg.ListarTodos();

            if (id.HasValue)
            {
                int valorId = (int)id;

                Livro = _livroNeg.ObterPorId(valorId);

                var listaAutoresPorid = _autorNeg.ListarCadastrados(valorId);
                AutoresCadastrados = listaAutoresPorid.Select(a => new SelectListItem
                {
                    Value = a.AutorId.ToString(),
                    Text = a.Nome
                }).ToList();


                var listaAssuntosPorId = _assuntoNeg.ListarCadastrados(valorId);
                AssuntosCadastrados = listaAssuntosPorId.Select(a => new SelectListItem
                {
                    Value = a.AssuntoId.ToString(),
                    Text = a.Descricao
                }).ToList();

             
                ListaLivroFormaPgto = _livroPgtoNeg.ObterPorId(valorId);
               

            }
            else
            {
                Livro = new LivroDto(); 
            }

        }

        public IActionResult OnGetExcluir(int id)
        {
            try
            {
                _livroNeg.Deletar(id); 
                TempData["Mensagem"] = "Livro excluído com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["Mensagem"] = $"Erro ao excluir o livro: {ex.Message}";
            }

            return RedirectToPage(); 
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                OnGet(0); 
                return Page();
            }

            if (Livro.LivroId > 0)
            {
                // Edição
                _livroNeg.Atualizar(Livro);
            }
            else
            {
                _livroNeg.SalvarLivroComRelacionamentos(Livro);
            }

           

            TempData["MensagemSucesso"] = "Livro cadastrado com sucesso!";           
            return RedirectToPage("./ListaLivros");
        }
    }

  

   
}
