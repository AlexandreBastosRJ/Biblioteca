namespace Biblioteca.Negocio.Dto;

public class LivroDto
{
    public int LivroId { get; set; }
    public string? Titulo { get; set; } = string.Empty;
    public string? Editora { get; set; } = string.Empty;
    public string? Edicao { get; set; } = string.Empty;
    public string? AnoPublicacao { get; set; } = string.Empty;
    public string? AssuntosSel { get; set; } = string.Empty;
    public string? AutoresSel { get; set; } = string.Empty;
    public string? FormasPgto { get; set; } = string.Empty;


    public List<int> AutoresIds { get; set; } = new();
    public List<int> AssuntosIds { get; set; } = new();

    public List<AutorDto> Autores { get; set; } = new(); 
    public List<AssuntoDto> Assuntos { get; set; } = new();
    public List<int> AutoresCadastrados { get; set; } = new();
    public List<int> AssuntosCadastrados { get; set; } = new();



}
