CREATE TYPE [dbo].[InputWord] AS TABLE (
    [wRank]    INT           NOT NULL,
    [wordName] NVARCHAR (50) NOT NULL,
    [typeId]   NCHAR (10)    NOT NULL);

