using Dapper.QX;

namespace FolderViewer.Blazor.Queries
{
    public class ChildFolderDataResult
    {
        public int RootId { get; set; }
        public string Name { get; set; }
        public long FolderSize { get; set; }
        public double PercentOfTotal { get; set; }
        public int FileCount { get; set; }
    }

    public class ChildFolderData : Query<ChildFolderDataResult>
    {
        public ChildFolderData() : base(
            @"WITH [total] AS (
                SELECT SUM([f].[Length]) AS [TotalSize]
                FROM [dbo].[File] [f]
                INNER JOIN [dbo].[FnFolderTree](@parentId) [tree] ON [f].[FolderId]=[tree].[Id]
            ), [detail] AS (
                SELECT
                    [dir].[Id] AS [RootId], [dir].[Name], [f].[Length]
                FROM
                    [dbo].[Folder] [dir]
                    CROSS APPLY [dbo].[FnFolderTree]([dir].[Id]) [subdirs]
                    INNER JOIN [dbo].[File] [f] ON [subdirs].[Id]=[f].[FolderId]
                WHERE
                    [dir].[ParentId]=@parentId

                UNION ALL

                SELECT
                    [f].[FolderId], '-- files --', [f].[Length]
                FROM
                    [dbo].[File] [f]
                WHERE
                    [f].[FolderId]=@parentId
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
                [g].[FolderSize] DESC")
        {
        }

        public int ParentId { get; set; }
    }
}
