using Dapper.QX;
using TreeData.Library.Models;

namespace FolderViewer.Blazor.Queries
{
    public class ChildFolders : Query<Folder>
    {
        public ChildFolders() : base(
            @"SELECT 
                [f].*,
                CASE 
                    WHEN EXISTS(SELECT 1 FROM [dbo].[Folder] WHERE [ParentId]=[f].[Id]) THEN 1
                    ELSE 0
                END AS [HasChildren]
            FROM 
                [dbo].[Folder] [f]
            WHERE 
                [ParentId]=@parentId AND
                NOT EXISTS(SELECT 1 FROM [dbo].[IgnoreFolder] WHERE [Name]=[f].[Name])
            ORDER BY 
                [Name]")
        {                
        }

        public int ParentId { get; set; }
    }
}
