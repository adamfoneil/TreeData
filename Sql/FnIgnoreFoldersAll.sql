CREATE FUNCTION [dbo].[FnIgnoreFoldersAll]() RETURNS @results TABLE (	
	[Id] int NOT NULL,
	[FullPath] nvarchar(max) NOT NULL,
	[Depth] int NOT NULL
) AS
BEGIN
	WITH [tree] AS (
		SELECT [dir].[Id], CAST([dir].[Name] as nvarchar(max)) AS [FullPath], 0 AS [Depth]
		FROM [dbo].[Folder] [dir] 
		INNER JOIN [dbo].[IgnoreFolder] [ig] ON [dir].[Name]=[ig].[Name]
		UNION ALL
		SELECT [child].[Id], CONCAT([tree].[FullPath], ' / ', [child].[Name]), [Depth] + 1
		FROM [tree]
		INNER JOIN [dbo].[Folder] [child] ON [tree].[Id]=[child].[ParentId]
	) INSERT INTO @results (
		[Id], [FullPath], [Depth]
	) SELECT
		[Id], [FullPath], [Depth]
	FROM
		[tree]

	RETURN
END
GO

SELECT [Id], COUNT(1)
FROM [dbo].[FnIgnoreFoldersAll]()
GROUP BY [Id]
HAVING (COUNT(1) > 1)

SELECT * FROM [dbo].[FnIgnoreFolders2]()