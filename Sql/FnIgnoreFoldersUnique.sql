CREATE FUNCTION [dbo].[FnIgnoreFoldersUnique]()
RETURNS @results TABLE (
	[Id] int PRIMARY KEY
) AS
BEGIN
	-- ignore explicitly named folders
	INSERT INTO @results ([Id])
    SELECT [dir].[Id]
    FROM [dbo].[Folder] [dir]
    INNER JOIN [dbo].[IgnoreFolder] [ig] ON [dir].[Name]=[ig].[Name];

	-- ignore the trees below these root folders
	INSERT INTO @results ([Id])
    SELECT [tree].[Id]
    FROM @results [ig]
    CROSS APPLY [dbo].[FnFolderTree]([ig].[Id]) [tree]
    WHERE NOT EXISTS(SELECT 1 FROM @results WHERE [Id]=[tree].[Id])
    GROUP BY [tree].[Id];

	RETURN
END
GO

DECLARE @ignore TABLE (
    [Id] int PRIMARY KEY
);

INSERT INTO @ignore ([Id])
--SELECT [Id] FROM [dbo].[FnIgnoreFolders2]()
SELECT [Id] FROM [dbo].[FnIgnoreFolders]()

SELECT * FROM @ignore