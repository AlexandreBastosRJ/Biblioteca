-- Usar a base de dados recém-criada
USE baseTestes;
GO

-- Criar a tabela Livro
CREATE TABLE Livro (
    Codl INT identity PRIMARY KEY,
    Titulo VARCHAR(40) NOT NULL,
    Editora VARCHAR(40) NOT NULL,
    Edicao INT NOT NULL,
    AnoPublicacao VARCHAR(4) NOT NULL
);
GO

-- Criar a tabela Autor
CREATE TABLE Autor (
    CodAu INT identity PRIMARY KEY,
    Nome VARCHAR(40) NOT NULL
);
GO

-- Criar a tabela de relacionamento Livro_Autor
CREATE TABLE Livro_Autor (
    Livro_Codl INT NOT NULL,
    Autor_CodAu INT NOT NULL,
    PRIMARY KEY (Livro_Codl, Autor_CodAu),
    FOREIGN KEY (Livro_Codl) REFERENCES Livro(Codl) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (Autor_CodAu) REFERENCES Autor(CodAu) ON DELETE CASCADE ON UPDATE CASCADE
);
GO

-- Criar a tabela Assunto
CREATE TABLE Assunto (
    CodAs INT identity PRIMARY KEY,
    Descricao VARCHAR(20) NOT NULL
);
GO

-- Criar a tabela de relacionamento Livro_Assunto
CREATE TABLE Livro_Assunto (
    Livro_Codl INT NOT NULL,
    Assunto_codAs INT NOT NULL,
    PRIMARY KEY (Livro_Codl, Assunto_codAs),
    FOREIGN KEY (Livro_Codl) REFERENCES Livro(Codl) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (Assunto_codAs) REFERENCES Assunto(CodAs) ON DELETE CASCADE ON UPDATE CASCADE
);
GO


CREATE TABLE FormaVenda (
    CodFv INT identity PRIMARY KEY,
    Descricao  VARCHAR(30) NOT NULL
);
GO


CREATE TABLE Livro_Valor_FormaVenda (
    Livro_Codl INT NOT NULL,
    FormaVenda_CodFv INT NOT NULL,
	Valor_Livro_FormaVenda Decimal (18,2),
    PRIMARY KEY (Livro_Codl, FormaVenda_CodFv),
    FOREIGN KEY (Livro_Codl) REFERENCES Livro(Codl) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (FormaVenda_CodFv) REFERENCES FormaVenda(CodFv) ON DELETE CASCADE ON UPDATE CASCADE
);
GO
