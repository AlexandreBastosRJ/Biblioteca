using AspNetCore.Reporting;
using Biblioteca.Negocio.Dto;
using Biblioteca.Negocio.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ClosedXML.Excel;
using System.IO;
using System.Globalization;
using Microsoft.Data.SqlClient;

namespace Biblioteca.Pages
{
    public class ListaLivrosModel : PageModel
    {
        private readonly ILivroNeg _livroNeg;
        private readonly IAutorNeg _autorNeg;
        private readonly IAssuntoNeg _assuntoNeg;
        private readonly IFormaPgtoNeg _formaPgtoNeg;
        private readonly ILivroFormaPgtoNeg _livroFormaPgtoNeg;

        public ListaLivrosModel(ILivroNeg livroNeg, IAutorNeg autorNeg, IAssuntoNeg assuntoNeg, IFormaPgtoNeg formaPgtoNeg,ILivroFormaPgtoNeg livroFormaPgtoNeg)
        {
            _livroNeg = livroNeg;
            _autorNeg = autorNeg;
            _assuntoNeg = assuntoNeg;
            _formaPgtoNeg = formaPgtoNeg;
            _livroFormaPgtoNeg = livroFormaPgtoNeg;

            Livros = new List<LivroDto>();
            Autores = new List<SelectListItem>();
            Assuntos = new List<SelectListItem>();
            FormasPgtoCadastradas = new List<FormaPgtoDto>();
            LivroFormasPgtoCadastradas = new List<LivroFormaPgtoDto>();

        }

        
        public List<LivroDto> Livros { get; set; }
        public List<FormaPgtoDto> FormasPgtoCadastradas { get; set; }
        public List<LivroFormaPgtoDto> LivroFormasPgtoCadastradas { get; set; }


        public List<SelectListItem> Autores { get; set; }
        public List<SelectListItem> Assuntos { get; set; }

     
        [BindProperty(SupportsGet = true)]
        public string? FiltroTitulo { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? FiltroAutorId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? FiltroAssuntoId { get; set; }

        public void OnGet()
        {
            FormasPgtoCadastradas = _formaPgtoNeg.ListarTodos();
            Autores = _autorNeg.ObterTodos()
                .Select(a => new SelectListItem { Value = a.AutorId.ToString(), Text = a.Nome })
                .ToList();

            Assuntos = _assuntoNeg.ListarTodos()
                .Select(a => new SelectListItem { Value = a.AssuntoId.ToString(), Text = a.Descricao })
                .ToList();

            // Busca com filtros
            Livros = _livroNeg.BuscarComFiltros(FiltroTitulo, FiltroAutorId, FiltroAssuntoId);
        }

        public IActionResult OnPostSalvarFormaPgto(int IdLivro, string FormaNegocio, string ValorFormaPgto)
        {
            try
            {
                string validaExiste = _livroFormaPgtoNeg.ValidaExiteFormaPgtonoLivro(IdLivro);
                if (validaExiste == "")
                {

                    decimal valorConvertido;
                    if (!decimal.TryParse(ValorFormaPgto, NumberStyles.Any, new CultureInfo("pt-BR"), out valorConvertido))
                    {
                        TempData["Erro"] = "Valor inválido. Use o formato 0,00.";
                        return Page();
                    }

                    var dados = new LivroFormaPgtoDto
                    {
                        Livro_Codl = IdLivro,
                        FormaVenda_CodFv = Int32.Parse(FormaNegocio),
                        Valor_Livro_FormaVenda = valorConvertido
                    };

                    _livroFormaPgtoNeg.SalvarLivroFormaPgto(dados);

                    TempData["Mensagem"] = "Forma de pagamento incluída com sucesso!";
                }
                else
                {
                    TempData["Mensagem"] = $"Já existe a forma de Pagamento {validaExiste}, para o Livro selecionado!";
                }
            }
            catch (SqlException)
            {
                TempData["Erro"] = "Erro ao cadastrar forma de pagamento.";
            }

            return RedirectToPage();
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

        public IActionResult OnPostImprimirRelatorioLivros()
        {
            // 1. Carrega os dados


            var listaLivros = _livroNeg.RelatorioLivros(); 

            // 2. Caminho do modelo
            var caminhoModelo = Path.Combine(Directory.GetCurrentDirectory(), "Pages", "Relatorio", "Livros.xlsx");

            // 3. Abre a planilha modelo
            using var workbook = new XLWorkbook(caminhoModelo);
            var worksheet = workbook.Worksheet(1); 

            int linha = 2;

            foreach (var livro in listaLivros)
            {
                worksheet.Cell(linha, 1).Value = livro.Titulo;
                worksheet.Cell(linha, 2).Value = livro.AutoresSel;
                worksheet.Cell(linha, 3).Value = livro.AssuntosSel;
                worksheet.Cell(linha, 4).Value = livro.Editora;
                worksheet.Cell(linha, 5).Value = livro.Edicao;
                worksheet.Cell(linha, 6).Value = livro.AnoPublicacao;  
                worksheet.Cell(linha, 7).Value = livro.FormaPgtoSel;
                linha++;
            }

            // 4. Exporta como arquivo para download
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "RelatorioLivros.xlsx");

        }
    }
}
