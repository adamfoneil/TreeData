using Dapper.QX;
using System;

namespace FolderViewer.Blazor.Queries
{
    public class FilesInAndBelowFolderResult
    {
        public string FullPath { get; set; }
        public int Id { get; set; }
        public int FolderId { get; set; }
        public string Name { get; set; }
        public long Length { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }

    public class FilesInAndBelowFolder : Query<FilesInAndBelowFolderResult>
    {
        public FilesInAndBelowFolder() : base(
            @"DECLARE @ignore TABLE (
                [Id] int PRIMARY KEY
            );

            -- ignore root items
            INSERT INTO @ignore ([Id])
            SELECT [dir].[Id]
            FROM [dbo].[Folder] [dir]
            INNER JOIN [dbo].[IgnoreFolder] [ig] ON [dir].[Name]=[ig].[Name];

            -- ignore children of those roots
            INSERT INTO @ignore ([Id])
            SELECT [tree].[Id]
            FROM @ignore [ig]
            CROSS APPLY [dbo].[FnFolderTree]([ig].[Id]) [tree]
            WHERE NOT EXISTS(SELECT 1 FROM @ignore WHERE [Id]=[tree].[Id])
            GROUP BY [tree].[Id];

            SELECT 
                [tree].[FullPath],    
                [f].*
            FROM 
                [dbo].[File] [f]
                INNER JOIN [dbo].[FnFolderTree](@folderId) [tree] ON [f].[FolderId]=[tree].[Id]	
            WHERE
                NOT EXISTS(SELECT 1 FROM @ignore WHERE [Id]=[f].[FolderId])
            ORDER BY
                [tree].[FullPath],
                [f].[Name]")
        {
        }

        public int FolderId { get; set; }
    }
}
