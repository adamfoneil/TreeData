-- 1. sanity check -- is my folder here?
SELECT * FROM [dbo].[Folder] WHERE [Name]='Drone';


-- 2. sample recursive query -- folders below "Drone", including "Drone"
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


-- 3. let's wrap this in a function for easier reuse
ALTER FUNCTION [dbo].[FnFolderTree](
	@rootFolderId int
) RETURNS @results TABLE (
	[Id] int NOT NULL,
	[FullPath] nvarchar(max) NOT NULL,
	[FolderName] nvarchar(50) NOT NULL,
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

-- 4. check our function
SELECT * FROM [dbo].[FnFolderTree](57125)


-- 5. now let's bring files into this
SELECT 
	[tree].*, [f].*
FROM 
	[dbo].[FnFolderTree](57125) [tree]
	INNER JOIN [dbo].[File] [f] ON [tree].[Id]=[f].[FolderId];


-- 6. let's check the total size
SELECT
	SUM([Length]) AS [TotalSize]
FROM (
	SELECT 
		[f].[Length]
	FROM 
		[dbo].[FnFolderTree](57125) [tree]
		INNER JOIN [dbo].[File] [f] ON [tree].[Id]=[f].[FolderId]
) AS [source]


-- 7a. immediate child folder size percentages
SELECT
	[child].[Name]
FROM
	[dbo].[Folder] [child]
WHERE
	[ParentId]=57125


-- 7b. folder names with total size in each folder
SELECT
	[child].[Name], SUM([f].[Length]) AS [FolderSize]
FROM
	[dbo].[Folder] [child]
	INNER JOIN [dbo].[File] [f] ON [child].[Id]=[f].[FolderId]
WHERE
	[ParentId]=57125
GROUP BY
	[child].[Name]


-- 7c. compare with total size to get percentages
DECLARE @rootId int
SET @rootId = 57125;

WITH [total] AS (
	SELECT SUM([f].[Length]) AS [TotalSize]
	FROM [dbo].[File] [f]
	INNER JOIN [dbo].[FnFolderTree](@rootId) [tree] ON [f].[FolderId]=[tree].[Id]
), [detail] AS (
	SELECT
		[tree].[FolderName], SUM([f].[Length]) AS [FolderSize]
	FROM
		[dbo].[FnFolderTree](@rootId) [tree]
		INNER JOIN [dbo].[File] [f] ON [tree].[Id]=[f].[FolderId]
	GROUP BY
		[tree].[FolderName]
) 
SELECT 
	[detail].*, [FolderSize] / CONVERT(float, [TotalSize]) AS [PercentOfTotal]
FROM
	[detail], [total]
-- whoops! this is every individual folder.... we want to collapse to immediate child folders


-- 7d. collapse sums to immediate children of "Drone" folder
DECLARE @rootId int
SET @rootId = 57125;

WITH [total] AS (
	SELECT SUM([f].[Length]) AS [TotalSize]
	FROM [dbo].[File] [f]
	INNER JOIN [dbo].[FnFolderTree](@rootId) [tree] ON [f].[FolderId]=[tree].[Id]
), [detail] AS (
	SELECT
		[dir].[Id] AS [RootId], [dir].[Name], [f].[Length]
	FROM
		[dbo].[Folder] [dir]
		CROSS APPLY [dbo].[FnFolderTree]([dir].[Id]) [subdirs]
		INNER JOIN [dbo].[File] [f] ON [subdirs].[Id]=[f].[FolderId]
	WHERE
		[dir].[ParentId]=@rootId

	UNION ALL

	SELECT
		[f].[FolderId], '-- files --', [f].[Length]
	FROM
		[dbo].[File] [f]
	WHERE
		[f].[FolderId]=@rootId
), [groups] AS (
	SELECT
		[detail].[RootId], [detail].[Name], SUM([detail].[Length]) AS [FolderSize], COUNT(1) AS [FileCount]
	FROM
		[detail]
	GROUP BY
		[detail].[RootId], [detail].[Name]
)
SELECT
	[g].[RootId], [g].[Name], [g].[FolderSize], [g].[FolderSize] / CONVERT(float, [t].[TotalSize]) AS [PercentOfTotal], [g].[FileCount]
FROM
	[groups] [g],
	[total] [t]
ORDER BY
	[g].[FolderSize] DESC


-- 8. troubleshoot file count in "Back Yard" -- OS reports 46, but my query reports 44
SELECT
	[f].*
FROM
	[dbo].[FnFolderTree](57126) [tree]
	INNER JOIN [dbo].[File] [f] ON [tree].[Id]=[f].[FolderId]
-- turned out to be missing "ALL" in "UNION ALL" above