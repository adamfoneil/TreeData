CREATE FUNCTION [dbo].[FnFolderRollup](
	@folderId int
) RETURNS @results TABLE (
	[FolderId] int NOT NULL,
	[Name] nvarchar(100) NOT NULL,
	[FolderSize] bigint NOT NULL,
	[PercentOfTotal] float NOT NULL,
	[FileCount] int NOT NULL
) AS
BEGIN
	WITH [total] AS (
		SELECT SUM([f].[Length]) AS [TotalSize]
		FROM [dbo].[File] [f]
		INNER JOIN [dbo].[FnFolderTree](@folderId) [tree] ON [f].[FolderId]=[tree].[Id]
	), [detail] AS (
		SELECT
			[dir].[Id] AS [RootId], [dir].[Name], [f].[Length]
		FROM
			[dbo].[Folder] [dir]
			CROSS APPLY [dbo].[FnFolderTree]([dir].[Id]) [subdirs]
			INNER JOIN [dbo].[File] [f] ON [subdirs].[Id]=[f].[FolderId]
		WHERE
			[dir].[ParentId]=@folderId AND
			NOT EXISTS(SELECT 1 FROM [dbo].[IgnoreFolder] WHERE [Name]=[dir].[Name])

		UNION ALL

		SELECT
			[f].[FolderId], '-- files --', [f].[Length]
		FROM
			[dbo].[File] [f]
		WHERE
			[f].[FolderId]=@folderId
	), [groups] AS (
		SELECT
			[detail].[RootId], [detail].[Name], SUM([detail].[Length]) AS [FolderSize], COUNT(1) AS [FileCount]
		FROM
			[detail]
		GROUP BY
			[detail].[RootId], [detail].[Name]
	)
	INSERT INTO @results (
		[FolderId], [Name], [FolderSize], [PercentOfTotal], [FileCount]
	) SELECT
		[g].[RootId], [g].[Name], [g].[FolderSize], [g].[FolderSize] / CONVERT(float, [t].[TotalSize]) AS [PercentOfTotal], [g].[FileCount]
	FROM
		[groups] [g],
		[total] [t]

	RETURN
END
GO

SELECT * FROM [dbo].[FnFolderRollup](57125)


SELECT * FROM [dbo].[FnFolderRollup](58152)
