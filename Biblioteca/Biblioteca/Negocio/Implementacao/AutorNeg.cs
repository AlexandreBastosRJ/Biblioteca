using Biblioteca.Negocio.Dto;
using Biblioteca.Negocio.Interface;
using Microsoft.Data.SqlClient;

namespace Biblioteca.Negocio.Implementacao
{
    public class AutorNeg : IAutorNeg
    {
        private readonly string _connectionString;

        public AutorNeg(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new ArgumentNullException(nameof(configuration), "String de conexão não encontrada.");
        }
        private static List<AutorDto> _listaAutor = new();

        public List<AutorDto> ObterTodos()
        {
            var lista = new List<AutorDto>();

            using var conexao = new SqlConnection(_connectionString);
            using var comando = new SqlCommand("SELECT CodAu, Nome FROM Autor", conexao);

            conexao.Open();
            using var reader = comando.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new AutorDto
                {
                    AutorId = reader.GetInt32(0),
                    Nome = reader.GetString(1)
                });
            }

            return lista;
        }

        public void Adicionar(AutorDto autor)
        {

            try
            {
                using var conexao = new SqlConnection(_connectionString);
                using var comando = new SqlCommand("INSERT INTO Autor (Nome) VALUES (@Nome)", conexao);
                comando.Parameters.AddWithValue("@Nome", autor.Nome);

                conexao.Open();
                comando.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro inesperado ao adicionar autor: {ex.Message}");
                throw;
            }
        }

        public AutorDto ObterPorId(int id)
        {
            using var conexao = new SqlConnection(_connectionString);
            var comando = new SqlCommand("SELECT CodAu, Nome FROM Autor WHERE CodAu = @Id", conexao);
            comando.Parameters.AddWithValue("@Id", id);

            conexao.Open();
            using var leitor = comando.ExecuteReader();
            if (leitor.Read())
            {
                return new AutorDto
                {
                    AutorId = leitor.GetInt32(0),
                    Nome = leitor.GetString(1)
                };
            }

            return null!;
        }

        public AutorDto ListarCadastrados(int id)
        {
            using var conexao = new SqlConnection(_connectionString);
            var comando = new SqlCommand("SELECT CodAu, Nome FROM Autor WHERE CodAu = @Id", conexao);
            comando.Parameters.AddWithValue("@Id", id);

            conexao.Open();
            using var leitor = comando.ExecuteReader();
            if (leitor.Read())
            {
                return new AutorDto
                {
                    AutorId = leitor.GetInt32(0),
                    Nome = leitor.GetString(1)
                };
            }

            return null!;
        }

        public void Excluir(int id)
        {
            using var conexao = new SqlConnection(_connectionString);
            var comando = new SqlCommand("DELETE FROM Autor WHERE CodAu = @Id", conexao);
            comando.Parameters.AddWithValue("@Id", id);

            conexao.Open();
            comando.ExecuteNonQuery();
        }

        public void Atualizar(AutorDto autor)
        {
            using var conexao = new SqlConnection(_connectionString);
            var comando = new SqlCommand("UPDATE Autor SET Nome = @Nome WHERE CodAu = @Id", conexao);
            comando.Parameters.AddWithValue("@Nome", autor.Nome);
            comando.Parameters.AddWithValue("@Id", autor.AutorId);

            conexao.Open();
            comando.ExecuteNonQuery();
        }

        List<AutorDto> IAutorNeg.ListarCadastrados(int id)
        {
            var lista = new List<AutorDto>();

            using var conexao = new SqlConnection(_connectionString);
            var comando = new SqlCommand("select a.CodAu,a.Nome from Livro_Autor La inner join autor A on a.CodAu = La.Autor_CodAu  where Livro_Codl = @Id", conexao);
            comando.Parameters.AddWithValue("@Id", id);

            conexao.Open();
            using var reader = comando.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new AutorDto
                {
                    AutorId = reader.GetInt32(0),
                    Nome = reader.GetString(1)
                });
            }

            return lista;
        }
    }
}
