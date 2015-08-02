CREATE TABLE [dbo].[Synonym] (
    [wordId1]   INT NOT NULL,
    [wordId2]   INT NULL,
    [synonymId] INT IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY CLUSTERED ([synonymId] ASC),
    CONSTRAINT [FK_Synonym_Word1] FOREIGN KEY ([wordId1]) REFERENCES [dbo].[Word] ([wordId]),
    CONSTRAINT [FK_Synonym_Word2] FOREIGN KEY ([wordId2]) REFERENCES [dbo].[Word] ([wordId])
);

