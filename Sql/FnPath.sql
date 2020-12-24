CREATE FUNCTION [dbo].[FnPath](
	@folderId int
) RETURNS @results TABLE (
	[Id] int NOT NULL,
	[ParentId] int NOT NULL,
	[Name] nvarchar(100) NOT NULL,
	[Depth] int NOT NULL
) AS
BEGIN
	WITH [tree] AS (
		SELECT [Id], [ParentId], [Name], 0 AS [Depth]
		FROM [dbo].[Folder]
		WHERE [Id]=@folderId
		UNION ALL
		SELECT [parent].[Id], [parent].[ParentId], [parent].[Name], [Depth]-1
		FROM [tree]
		INNER JOIN [dbo].[Folder] [parent] ON [tree].[ParentId]=[parent].[Id]
	) INSERT INTO @results (
		[Id], [ParentId], [Name], [Depth]
	) SELECT
		[Id], [ParentId], [Name], [Depth]
	FROM
		[tree]

	RETURN
END
GO

SELECT * FROM [dbo].[FnPath](57134)