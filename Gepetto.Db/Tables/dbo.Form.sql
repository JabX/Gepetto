CREATE TABLE [dbo].[Form] (
    [wordId]   INT           NOT NULL,
    [formName] NVARCHAR (50) NOT NULL,
    [formId]   INT           IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY CLUSTERED ([formId] ASC),
    CONSTRAINT [FK_Form_Word] FOREIGN KEY ([wordId]) REFERENCES [dbo].[Word] ([wordId])
);

