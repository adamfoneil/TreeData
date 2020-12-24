CREATE FUNCTION [dbo].[FnFolderTree](
	@rootFolderId int
) RETURNS @results TABLE (
	[Id] int NOT NULL,
	[FullPath] nvarchar(max) NOT NULL,
	[FolderName] nvarchar(100) NOT NULL,
	[Depth] int NOT NULL
) AS
BEGIN
	WITH [tree] AS (
		SELECT [Id], CAST([Name] AS nvarchar(max)) AS [FullPath], [Name], 0 AS [Depth]
		FROM [dbo].[Folder]
		WHERE [Id]=@rootFolderId
		UNION ALL
		SELECT [child].[Id], CONCAT([tree].[FullPath], ' / ' + [child].[Name]) AS [FullPath], [child].[Name], [tree].[Depth] + 1
		FROM [tree] 
		INNER JOIN [dbo].[Folder] [child] ON [child].[ParentId]=[tree].[Id]
	) INSERT INTO @results (
		[Id], [FullPath], [FolderName], [Depth]
	) SELECT
		[Id], [FullPath], [Name], [Depth]
	FROM
		[tree]

	RETURN
END
GO

SELECT * FROM [Folder] WHERE [ParentId]=0

SELECT * FROM [dbo].[FnFolderTree](57001)

SELECT * FROM [dbo].[FnFolderTree](78240)

SELECT * FROM [Folder] WHERE [Name]='Drone'

SELECT * FROM [dbo].[FnFolderTree](57125)