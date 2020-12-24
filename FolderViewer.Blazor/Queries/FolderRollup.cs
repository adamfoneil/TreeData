using Dapper.QX;

namespace FolderViewer.Blazor.Queries
{
    public class FolderRollupResult
    {
        public int FolderId { get; set; }
        public string Name { get; set; }
        public long FolderSize { get; set; }
        public double PercentOfTotal { get; set; }
        public int FileCount { get; set; }
    }

    public class FolderRollup : Query<FolderRollupResult>
    {
        public FolderRollup() : base("SELECT * FROM [dbo].[FnFolderRollup](@parentId) ORDER BY [Name]")
        {
        }

        public int ParentId { get; set; }
    }
}
