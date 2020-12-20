
-- 1. sanity check -- is my folder here?
SELECT * FROM [dbo].[Folder] WHERE [Name]='Drone';
-- yes!

-- 2. sample recursive query
WITH [tree] AS (
	SELECT [Id], CAST([Name] AS nvarchar(max)) AS [FullPath], 0 AS [Depth]
	FROM [dbo].[Folder]
	WHERE [Name]='Drone'
	UNION ALL
	SELECT [child].[Id], CONCAT([tree].[FullPath], ' / ' + [child].[Name]) AS [FullPath], [tree].[Depth] + 1
	FROM [tree] 
	INNER JOIN [dbo].[Folder] [child] ON [child].[ParentId]=[tree].[Id]
) 
SELECT * FROM [tree];
GO

-- 3. let's wrap this in a function for easier reuse
ALTER FUNCTION [dbo].[FnFolderTree](
	@rootFolderId int
) RETURNS @results TABLE (
	[Id] int NOT NULL,
	[FullPath] nvarchar(max) NOT NULL,
	[Depth] int NOT NULL
) AS
BEGIN
	WITH [tree] AS (
		SELECT [Id], CAST([Name] AS nvarchar(max)) AS [FullPath], 0 AS [Depth]
		FROM [dbo].[Folder]
		WHERE [Id]=@rootFolderId
		UNION ALL
		SELECT [child].[Id], CONCAT([tree].[FullPath], ' / ' + [child].[Name]) AS [FullPath], [tree].[Depth] + 1
		FROM [tree] 
		INNER JOIN [dbo].[Folder] [child] ON [child].[ParentId]=[tree].[Id]
	) INSERT INTO @results (
		[Id], [FullPath], [Depth]
	) SELECT
		[Id], [FullPath], [Depth]
	FROM
		[tree]

	RETURN
END

-- 4. now let's bring files into this

SELECT 
	[tree].*, [f].*
FROM 
	[dbo].[FnFolderTree](57125) [tree]
	INNER JOIN [dbo].[File] [f] ON [tree].[Id]=[f].[FolderId]