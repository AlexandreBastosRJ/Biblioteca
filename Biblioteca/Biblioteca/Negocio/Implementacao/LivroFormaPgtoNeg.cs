using Biblioteca.Negocio.Dto;
using Biblioteca.Negocio.Interface;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection.PortableExecutable;

namespace Biblioteca.Negocio.Implementacao
{
    public class LivroFormaPgtoNeg : ILivroFormaPgtoNeg
    {
        private readonly string _connectionString;

        public LivroFormaPgtoNeg(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new ArgumentNullException(nameof(configuration), "String de conexão não encontrada.");
        }
        private static List<LivroFormaPgtoDto> _listaLivroFormaPgto = new();

        public void Atualizar(LivroFormaPgtoDto livroFormaPgto)
        {
            throw new NotImplementedException();
        }

        public void Deletar(int id)
        {
            throw new NotImplementedException();
        }

        public List<LivroFormaPgtoDto> ObterPorId(int id)
        {
            var lista = new List<LivroFormaPgtoDto>();

            using var conexao = new SqlConnection(_connectionString);
            var comando = new SqlCommand(@"
                SELECT 
                    Livro_Codl, FormaVenda_CodFv,
                    Descricao,
                    Valor_Livro_FormaVenda 
                FROM  FormaVenda FV 
                INNER JOIN Livro_Valor_FormaVenda LFv ON FV.CodFv = LFv.FormaVenda_CodFv 
                WHERE Livro_Codl = @Id", conexao);

            comando.Parameters.AddWithValue("@Id", id);
            conexao.Open();

            using var leitor = comando.ExecuteReader();
            while (leitor.Read())
            {
                lista.Add(new LivroFormaPgtoDto
                {
                    Livro_Codl = !leitor.IsDBNull(0) ? leitor.GetInt32(0) : 0,
                    FormaVenda_CodFv = !leitor.IsDBNull(1) ? leitor.GetInt32(1) : 0,
                    DescricaoFormaPgto = !leitor.IsDBNull(2) ? leitor.GetValue(2)?.ToString() ?? string.Empty: null,
                    Valor_Livro_FormaVenda = leitor.IsDBNull(3) ? null : leitor.GetDecimal(3)
                });
            }

            return lista;
        }


        public void SalvarLivroFormaPgto(LivroFormaPgtoDto livroFormaPgtoDto)
        {
            try
            {
                using var conexao = new SqlConnection(_connectionString);
                using var comando = new SqlCommand("INSERT INTO Livro_Valor_FormaVenda (Livro_Codl,FormaVenda_CodFv,Valor_Livro_FormaVenda) VALUES (@Livro_Codl,@FormaVenda_CodFv, @Valor_Livro_FormaVenda)", conexao);
                comando.Parameters.AddWithValue("@Livro_Codl", livroFormaPgtoDto.Livro_Codl);
                comando.Parameters.AddWithValue("@FormaVenda_CodFv", livroFormaPgtoDto.FormaVenda_CodFv);
                comando.Parameters.AddWithValue("@Valor_Livro_FormaVenda", livroFormaPgtoDto.Valor_Livro_FormaVenda);

                conexao.Open();
                comando.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Erro inesperado: {ex.Message}");
                throw;
            }
        }

        public string ValidaExiteFormaPgtonoLivro(int id)
        {
            string formaPgto = "";


            using var conexao = new SqlConnection(_connectionString);
            var comando = new SqlCommand(" select  descricao from FormaVenda Fv inner join Livro_Valor_FormaVenda LVF on Fv.CodFv = @Id", conexao);
            comando.Parameters.AddWithValue("@Id", id);

            conexao.Open();
            using var reader = comando.ExecuteReader();

            if (reader.Read())
            {
                formaPgto = reader.GetString(0);
            }

            return formaPgto;
        }
    }
}
