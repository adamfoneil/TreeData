using Dapper.QX;
using Dapper.QX.Attributes;
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

            INSERT INTO @ignore ([Id])
            SELECT [Id] FROM [dbo].[FnIgnoreFoldersUnique]()

            SELECT 
                [tree].[FullPath],    
                [f].*
            FROM 
                [dbo].[File] [f]
                INNER JOIN [dbo].[FnFolderTree](@folderId) [tree] ON [f].[FolderId]=[tree].[Id]	
            WHERE
                NOT EXISTS(SELECT 1 FROM @ignore WHERE [Id]=[f].[FolderId]) {andWhere}                
            ORDER BY
                [tree].[FullPath],
                [f].[Name]")
        {
        }

        public int FolderId { get; set; }

        [Where("[Depth]<=@depth")]
        public int? Depth { get; set; }
    }
}
