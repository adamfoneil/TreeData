using Dapper.QX;
using Dapper.QX.Attributes;

namespace FolderViewer.Blazor.Queries
{
    public class FolderTreeResult
    {
        public int Id { get; set; }
        public string FullPath { get; set; }
        public string FolderName { get; set; }
        public int ParentId { get; set; }
        public int Depth { get; set; }
    }

    public class FolderTree : Query<FolderTreeResult>
    {
        public FolderTree() : base("SELECT * FROM [dbo].[FnFolderTree](@folderId) {where}")
        {
        }

        public int FolderId { get; set; }

        [Where("[Depth]=@depth")]
        public int? Depth { get; set; }
    }
}
