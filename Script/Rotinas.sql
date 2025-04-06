select * from Livro

select a.CodAu,a.Nome from Livro_Autor La inner join autor A on a.CodAu = La.Autor_CodAu  where Livro_Codl = 7

select * from Livro_Assunto


SELECT 
    L.Codl, 
    L.Titulo, 
    L.Editora, 
    L.Edicao, 
    L.AnoPublicacao,
    STUFF((
        SELECT ', ' + AutInterno.Nome
        FROM Livro_Autor LAInterno
        INNER JOIN Autor AutInterno ON LAInterno.Autor_CodAu = AutInterno.CodAu
        WHERE LAInterno.Livro_Codl = L.Codl
        FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS Autores,
	STUFF((
        SELECT ', ' + AssInterno.Descricao
        FROM Livro_Assunto LAInterno
        INNER JOIN Assunto AssInterno ON LAInterno.Assunto_codAs = AssInterno.CodAs
        WHERE LAInterno.Livro_Codl = L.Codl
        FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS Assuntos
FROM Livro L
LEFT JOIN Livro_Autor LA ON LA.Livro_Codl = L.Codl
LEFT JOIN Livro_Assunto LS ON LS.Livro_Codl = L.Codl
WHERE L.Codl = 7
AND L.Titulo LIKE '%Mem%'
GROUP BY 
    L.Codl, 
    L.Titulo, 
    L.Editora, 
    L.Edicao, 
    L.AnoPublicacao;



	  SELECT 
            L.Codl, 
            L.Titulo, 
            L.Editora, 
            L.Edicao, 
            L.AnoPublicacao,
            STUFF((
                SELECT ', ' + AutInterno.Nome
                FROM Livro_Autor LAInterno
                INNER JOIN Autor AutInterno ON LAInterno.Autor_CodAu = AutInterno.CodAu
                WHERE LAInterno.Livro_Codl = L.Codl
                FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS AutoresSel,
            STUFF((
                SELECT ', ' + AssInterno.Descricao
                FROM Livro_Assunto LAInterno
                INNER JOIN Assunto AssInterno ON LAInterno.Assunto_codAs = AssInterno.CodAs
                WHERE LAInterno.Livro_Codl = L.Codl
                FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS AssuntosSel,
			STUFF((
                SELECT '| ' + Fv.Descricao + ' - ' + cast(LVfv.Valor_Livro_FormaVenda as varchar) 
                FROM Livro_Valor_FormaVenda LVfv
                INNER JOIN FormaVenda Fv ON Fv.CodFv = LVfv.FormaVenda_CodFv
                WHERE LVfv.Livro_Codl = L.Codl
                FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS FormaPgtoSel
        FROM Livro L
    


		select a.CodAu,a.Nome from Livro_Autor La inner join autor A on a.CodAu = La.Autor_CodAu  where Livro_Codl = 7

		select a.CodAs,a.Descricao from Livro_Assunto La inner join Assunto A on a.CodAs = La.Assunto_codAs  where Livro_Codl = 7

	

select * from Livro


 SELECT 
     L.Codl, 
     L.Titulo, 
     L.Editora, 
    cast( L.Edicao as varchar ) as Edicao, 
     cast( L.AnoPublicacao  as varchar ) asAnoPublicacao ,
     STUFF((
         SELECT ', ' + AutInterno.Nome
         FROM Livro_Autor LAInterno
         INNER JOIN Autor AutInterno ON LAInterno.Autor_CodAu = AutInterno.CodAu
         WHERE LAInterno.Livro_Codl = L.Codl
         FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS AutoresSel,
     STUFF((
         SELECT ', ' + AssInterno.Descricao
         FROM Livro_Assunto LAInterno
         INNER JOIN Assunto AssInterno ON LAInterno.Assunto_codAs = AssInterno.CodAs
         WHERE LAInterno.Livro_Codl = L.Codl
         FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS AssuntosSel,
     STUFF((
         SELECT '| ' + Fv.Descricao + ' - ' + CAST(LVfv.Valor_Livro_FormaVenda AS VARCHAR)
         FROM Livro_Valor_FormaVenda LVfv
         INNER JOIN FormaVenda Fv ON Fv.CodFv = LVfv.FormaVenda_CodFv
         WHERE LVfv.Livro_Codl = L.Codl
         FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS FormaPgtoSel
 FROM Livro L