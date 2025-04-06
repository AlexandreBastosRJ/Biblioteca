using Biblioteca.Negocio.Dto;
using Biblioteca.Negocio.Interface;
using Microsoft.Data.SqlClient;

namespace Biblioteca.Negocio.Implementacao
{
    public class LivroNeg : ILivroNeg
    {
        private readonly string _connectionString;

        public LivroNeg(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new ArgumentNullException(nameof(configuration), "String de conexão não encontrada.");
        }
        private static List<LivroDto> _listaLivros = new();


        public List<LivroDto> BuscarComFiltros(string? titulo, int? autorId, int? assuntoId)
        {
            var lista = new List<LivroDto>();

            using var conexao = new SqlConnection(_connectionString);
            using var comando = new SqlCommand();
            comando.Connection = conexao;

            var sql = @"
                        SELECT 
                            Codl,
                            Titulo,
                            Editora,
                            Edicao,
                            AnoPublicacao,
                            AutoresSel,
                            AssuntosSel,
                            FormaPgtoSel
                        from vw_LivrosDetalhados
                        WHERE 1= 1";
                  

            if (!string.IsNullOrWhiteSpace(titulo))
            {
                sql += " AND Titulo LIKE @Titulo";
                comando.Parameters.AddWithValue("@Titulo", $"%{titulo}%");
            }

            if (autorId.HasValue)
            {
                sql += " AND AutoresSel = @AutorId";
                comando.Parameters.AddWithValue("@AutorId", autorId.Value);
            }

            if (assuntoId.HasValue)
            {
                sql += " AND AssuntosSel = @AssuntoId";
                comando.Parameters.AddWithValue("@AssuntoId", assuntoId.Value);
            }

          


            comando.CommandText = sql;

            conexao.Open();
            using var reader = comando.ExecuteReader();

            while (reader.Read())
            {
                var livro = new LivroDto
                {
                    LivroId = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                    Titulo = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    Editora = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    Edicao = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    AnoPublicacao = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                    AutoresSel = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                    AssuntosSel = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                    FormasPgto = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                    Autores = new List<AutorDto>(),
                    Assuntos = new List<AssuntoDto>()
                };

                lista.Add(livro);
            }


            foreach (var livro in lista)
            {
                livro.Autores = ObterAutoresPorLivroId(livro.LivroId);
                livro.Assuntos = ObterAssuntosPorLivroId(livro.LivroId);
            }

            return lista;
        }

        private List<AutorDto> ObterAutoresPorLivroId(int livroId)
        {
            var lista = new List<AutorDto>();

            using var conexao = new SqlConnection(_connectionString);
            using var comando = new SqlCommand(@"
        SELECT A.CodAu, A.Nome
        FROM Autor A
        INNER JOIN Livro_Autor LA ON A.CodAu = LA.Autor_CodAu
        WHERE LA.Livro_Codl = @LivroId", conexao);

            comando.Parameters.AddWithValue("@LivroId", livroId);

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

        private List<AssuntoDto> ObterAssuntosPorLivroId(int livroId)
        {
            var lista = new List<AssuntoDto>();

            using var conexao = new SqlConnection(_connectionString);
            using var comando = new SqlCommand(@"
        SELECT S.CodAs, S.Descricao
        FROM Assunto S
        INNER JOIN Livro_Assunto LS ON S.CodAs = LS.Assunto_codAs
        WHERE LS.Livro_Codl = @LivroId", conexao);

            comando.Parameters.AddWithValue("@LivroId", livroId);

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


        public void SalvarLivroComRelacionamentos(LivroDto livro)
        {
            try
            {
                using var conexao = new SqlConnection(_connectionString);
                conexao.Open();

                using var transacao = conexao.BeginTransaction();

                try
                {
                    // 1. Inserir o livro
                    var inserirLivroQuery = @"
                INSERT INTO Livro (Titulo, Editora, Edicao, AnoPublicacao)
                VALUES (@Titulo, @Editora, @Edicao, @AnoPublicacao);
                SELECT SCOPE_IDENTITY();
            ";

                    using var cmdLivro = new SqlCommand(inserirLivroQuery, conexao, transacao);
                    cmdLivro.Parameters.AddWithValue("@Titulo", livro.Titulo);
                    cmdLivro.Parameters.AddWithValue("@Editora", livro.Editora);
                    cmdLivro.Parameters.AddWithValue("@Edicao", livro.Edicao);
                    cmdLivro.Parameters.AddWithValue("@AnoPublicacao", livro.AnoPublicacao);

                    var livroIdObj = cmdLivro.ExecuteScalar();
                    int livroId = Convert.ToInt32(livroIdObj);

                    // 2. Inserir os autores relacionados
                    foreach (var autorId in livro.AutoresIds)
                    {
                        var insertAutorRel = @"
                    INSERT INTO Livro_Autor (Livro_Codl, Autor_CodAu)
                    VALUES (@LivroId, @AutorId);
                ";

                        using var cmdAutor = new SqlCommand(insertAutorRel, conexao, transacao);
                        cmdAutor.Parameters.AddWithValue("@LivroId", livroId);
                        cmdAutor.Parameters.AddWithValue("@AutorId", autorId);
                        cmdAutor.ExecuteNonQuery();
                    }

                    // 3. Inserir os assuntos relacionados
                    foreach (var assuntoId in livro.AssuntosIds)
                    {
                        var insertAssuntoRel = @"
                    INSERT INTO Livro_Assunto (Livro_Codl, Assunto_codAs)
                    VALUES (@LivroId, @AssuntoId);
                ";

                        using var cmdAssunto = new SqlCommand(insertAssuntoRel, conexao, transacao);
                        cmdAssunto.Parameters.AddWithValue("@LivroId", livroId);
                        cmdAssunto.Parameters.AddWithValue("@AssuntoId", assuntoId);
                        cmdAssunto.ExecuteNonQuery();
                    }

                    // 4. Confirma a transação
                    transacao.Commit();
                }
                catch (Exception ex)
                {
                    transacao.Rollback();
                    Console.WriteLine($"Erro ao salvar livro com relacionamentos: {ex.Message}");
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro de conexão: {ex.Message}");
                throw;
            }
        }

        public void Atualizar(LivroDto livro)
        {
            // Lógica aqui
        }

        public void Deletar(int id)
        {
            using var conexao = new SqlConnection(_connectionString);
            conexao.Open();


            // Exclui da tabela de relacionamento Livro_Assunto
            using (var cmdLivroAssunto = new SqlCommand("DELETE FROM Livro_Assunto WHERE Livro_Codl = @Id", conexao))
            {
                cmdLivroAssunto.Parameters.AddWithValue("@Id", id);
                cmdLivroAssunto.ExecuteNonQuery();
            }

            // Exclui da tabela de relacionamento Livro_Autor
            using (var cmdLivroAutor = new SqlCommand("DELETE FROM Livro_Autor WHERE Livro_Codl = @Id", conexao))
            {
                cmdLivroAutor.Parameters.AddWithValue("@Id", id);
                cmdLivroAutor.ExecuteNonQuery();
            }

            // Exclui da tabela principal Livro
            using (var cmdLivro = new SqlCommand("DELETE FROM Livro WHERE Codl = @Id", conexao))
            {
                cmdLivro.Parameters.AddWithValue("@Id", id);
                cmdLivro.ExecuteNonQuery();
            }
        }

        public LivroDto ObterPorId(int id)
        {
            LivroDto? livro = null;

            using var conexao = new SqlConnection(_connectionString);
            using var comando = new SqlCommand();
            comando.Connection = conexao;

            var sql = @"
                        SELECT 
                            Codl,
                            Titulo,
                            Editora,
                            Edicao,
                            AnoPublicacao,
                            AutoresSel,
                            AssuntosSel,
                            FormaPgtoSel
                        from vw_LivrosDetalhados
                        WHERE Codl = @Id";

            comando.CommandText = sql;
            comando.Parameters.AddWithValue("@Id", id);

            conexao.Open();
            using var reader = comando.ExecuteReader();

            if (reader.Read())
            {
                livro = new LivroDto
                {
                    LivroId = reader.GetInt32(0),
                    Titulo = reader.IsDBNull(1) ? null : reader.GetString(1),
                    Editora = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Edicao = reader.IsDBNull(3) ? null : reader.GetString(3),
                    AnoPublicacao = reader.IsDBNull(4) ? null : reader.GetString(4),
                    AutoresSel = reader.IsDBNull(5) ? null : reader.GetString(5),
                    AssuntosSel = reader.IsDBNull(6) ? null : reader.GetString(6),
                    Autores = ObterAutoresPorLivroId(id),
                    Assuntos = ObterAssuntosPorLivroId(id)

                };

                // Preencher os Ids dos autores e assuntos (para pré-selecionar no <select>)
                livro.AutoresIds = livro.Autores.Select(a => a.AutorId).ToList();
                livro.AssuntosIds = livro.Assuntos.Select(a => a.AssuntoId).ToList();


            }

            return livro!;
        }

        public IEnumerable<LivroDto> ObterTodos()
        {
            // Lógica aqui
            return new List<LivroDto>(); // Exemplo
        }

        public List<RelatorioLivrosDto> RelatorioLivros()
        {
            var livros = new List<RelatorioLivrosDto>();

            using var conexao = new SqlConnection(_connectionString);
            using var comando = new SqlCommand();
            comando.Connection = conexao;

            var sql = @"
                        SELECT 
                            Codl,
                            Titulo,
                            Editora,
                            Edicao,
                            AnoPublicacao,
                            AutoresSel,
                            AssuntosSel,
                            FormaPgtoSel
                        from vw_LivrosDetalhados";

            comando.CommandText = sql;

            conexao.Open();
            using var reader = comando.ExecuteReader();

            while (reader.Read())
            {
                var livro = new RelatorioLivrosDto
                {
                    Titulo = reader.IsDBNull(1) ? null : reader.GetString(1),
                    Editora = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Edicao = reader.IsDBNull(3) ? null : reader.GetString(3),
                    AnoPublicacao = reader.IsDBNull(4) ? null : reader.GetString(4),
                    AutoresSel = reader.IsDBNull(5) ? null : reader.GetString(5),
                    AssuntosSel = reader.IsDBNull(6) ? null : reader.GetString(6),
                    FormaPgtoSel = reader.IsDBNull(7) ? null : reader.GetString(7)
                };

                livros.Add(livro);
            }

            return livros;
        }

    }
}
