using Dapper.QX;
using System.Threading.Tasks;

namespace TreeData.Library.Queries
{
    public class GetPathResult
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Name { get; set; }
        public int Depth { get; set; }
    }

    public class GetPath : Query<GetPathResult>
    {
        public GetPath() : base("SELECT * FROM [dbo].[FnPath](@folderId) ORDER BY [Depth] ASC")
        {
        }

        public int FolderId { get; set; }
    }
}
