using Biblioteca.Negocio.Dto;
using Biblioteca.Negocio.Interface;
using Microsoft.Data.SqlClient;

namespace Biblioteca.Negocio.Implementacao
{
    public class FormaPgtoNeg : IFormaPgtoNeg
    {
        private readonly string _connectionString;

        public FormaPgtoNeg(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new ArgumentNullException(nameof(configuration), "String de conexão não encontrada.");
        }

        public List<FormaPgtoDto> ListarTodos()
        {
            var lista = new List<FormaPgtoDto>();

            using var conexao = new SqlConnection(_connectionString);
            using var comando = new SqlCommand("SELECT CodFv, Descricao FROM FormaVenda", conexao);

            conexao.Open();
            using var reader = comando.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new FormaPgtoDto
                {
                    CodFv = reader.GetInt32(0),
                    Descricao = reader.GetString(1)
                });
            }

            return lista;
        }

        public FormaPgtoDto ObterPorId(int id)
        {
            using var conexao = new SqlConnection(_connectionString);
            var comando = new SqlCommand("SELECT CodFv, Descricao FROM FormaVenda WHERE CodFv = @Id", conexao);
            comando.Parameters.AddWithValue("@Id", id);

            conexao.Open();
            using var reader = comando.ExecuteReader();
            if (reader.Read())
            {
                return new FormaPgtoDto
                {
                    CodFv = reader.GetInt32(0),
                    Descricao = reader.GetString(1)
                };
            }

            return null!;
        }

        public void Adicionar(FormaPgtoDto dto)
        {
            try
            {
                using var conexao = new SqlConnection(_connectionString);
                using var comando = new SqlCommand("INSERT INTO FormaVenda (Descricao) VALUES (@Descricao)", conexao);
                comando.Parameters.AddWithValue("@Descricao", dto.Descricao);

                conexao.Open();
                comando.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao adicionar forma de pagamento: {ex.Message}");
                throw;
            }
        }

        public void Atualizar(FormaPgtoDto dto)
        {
            using var conexao = new SqlConnection(_connectionString);
            var comando = new SqlCommand("UPDATE FormaVenda SET Descricao = @Descricao WHERE CodFv = @Id", conexao);
            comando.Parameters.AddWithValue("@Descricao", dto.Descricao);
            comando.Parameters.AddWithValue("@Id", dto.CodFv);

            conexao.Open();
            comando.ExecuteNonQuery();
        }

        public void Excluir(int id)
        {
            using var conexao = new SqlConnection(_connectionString);
            var comando = new SqlCommand("DELETE FROM FormaVenda WHERE CodFv = @Id", conexao);
            comando.Parameters.AddWithValue("@Id", id);

            conexao.Open();
            comando.ExecuteNonQuery();
        }
    }
}
