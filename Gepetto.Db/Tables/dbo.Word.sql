CREATE TABLE [dbo].[Word] (
    [wordName] NVARCHAR (50) NOT NULL,
    [typeId]   NCHAR (3)    NOT NULL,
    [wordId]   INT           IDENTITY (1, 1) NOT NULL,
    [gScore]   INT           NOT NULL,
    PRIMARY KEY CLUSTERED ([wordId] ASC),
    CONSTRAINT [fk_word_wordType] FOREIGN KEY ([typeId]) REFERENCES [dbo].[WordType] ([typeId])
);

