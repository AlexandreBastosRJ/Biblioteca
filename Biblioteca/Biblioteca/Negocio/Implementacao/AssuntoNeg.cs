using Biblioteca.Negocio.Dto;
using Biblioteca.Negocio.Interface;
using Microsoft.Data.SqlClient;

namespace Biblioteca.Negocio.Implementacao
{
    public class AssuntoNeg : IAssuntoNeg
    {
        private readonly string _connectionString;

        public AssuntoNeg(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new ArgumentNullException(nameof(configuration), "String de conexão não encontrada.");
        }
        private static List<AssuntoDto> _listaAssuntos = new();

        public List<AssuntoDto> ListarTodos()
        {
            var lista = new List<AssuntoDto>();

            using var conexao = new SqlConnection(_connectionString);
            using var comando = new SqlCommand("SELECT CodAs, Descricao FROM Assunto", conexao);

            conexao.Open();
            using var reader = comando.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new AssuntoDto
                {
                    AssuntoId = reader.GetInt32(0),  
                    Descricao = reader.GetString(1)
                });
            }

            return lista;
        }

        public List<AssuntoDto> ListarCadastrados(int id)
        {
            var lista = new List<AssuntoDto>();

            using var conexao = new SqlConnection(_connectionString);
            var comando = new SqlCommand("SELECT CodAs, Descricao FROM Assunto WHERE CodAs = @Id", conexao);
            comando.Parameters.AddWithValue("@Id", id);

            conexao.Open();
            using var reader = comando.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new AssuntoDto
                {
                    AssuntoId = reader.GetInt32(0),
                    Descricao = reader.GetString(1)
                });
            }

            return lista;
        }

        public void Adicionar(AssuntoDto assunto)
        {

            try
            {
                using var conexao = new SqlConnection(_connectionString);
                using var comando = new SqlCommand("INSERT INTO Assunto (Descricao) VALUES (@Descricao)", conexao);
                comando.Parameters.AddWithValue("@Descricao", assunto.Descricao);

                conexao.Open();
                comando.ExecuteNonQuery();
            }
            
            catch (Exception ex)
            {
                Console.WriteLine($"Erro inesperado: {ex.Message}");
                throw;
            }
        }
        public AssuntoDto ObterPorId(int id)
        {
            using var conexao = new SqlConnection(_connectionString);
            var comando = new SqlCommand("SELECT CodAs, Descricao FROM Assunto WHERE CodAs = @Id", conexao);
            comando.Parameters.AddWithValue("@Id", id);

            conexao.Open();
            using var leitor = comando.ExecuteReader();
            if (leitor.Read())
            {
                return new AssuntoDto
                {
                    AssuntoId = leitor.GetInt32(0),
                    Descricao = leitor.GetString(1)
                };
            }

            return null!;
        }

        public void Excluir(int id)
        {
            using var conexao = new SqlConnection(_connectionString);
            var comando = new SqlCommand("DELETE FROM Assunto WHERE CodAs = @Id", conexao);
            comando.Parameters.AddWithValue("@Id", id);

            conexao.Open();
            comando.ExecuteNonQuery();
        }

        public void Atualizar(AssuntoDto assunto)
        {
            using var conexao = new SqlConnection(_connectionString);
            var comando = new SqlCommand("UPDATE Assunto SET Descricao = @Descricao WHERE CodAs = @Id", conexao);
            comando.Parameters.AddWithValue("@Descricao", assunto.Descricao);
            comando.Parameters.AddWithValue("@Id", assunto.AssuntoId);

            conexao.Open();
            comando.ExecuteNonQuery();
        }

        public IEnumerable<AssuntoDto> ListarCadastrados(int? id)
        {
            var lista = new List<AssuntoDto>();

            using var conexao = new SqlConnection(_connectionString);
            var comando = new SqlCommand("\tselect a.CodAs,a.Descricao from Livro_Assunto La inner join Assunto A on a.CodAs = La.Assunto_codAs  where Livro_Codl = @Id", conexao);
            comando.Parameters.AddWithValue("@Id", id);

            conexao.Open();
            using var reader = comando.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new AssuntoDto
                {
                    AssuntoId = reader.GetInt32(0),
                    Descricao = reader.GetString(1)
                });
            }

            return lista;
        }
    }
}
