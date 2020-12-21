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
            @"SELECT 
                [tree].[FullPath],    
                [f].*
            FROM 
                [dbo].[File] [f]
                INNER JOIN [dbo].[FnFolderTree](@folderId) [tree] ON [f].[FolderId]=[tree].[Id]
            ORDER BY
                [f].[Name]")
        {
        }

        public int FolderId { get; set; }
    }
}
