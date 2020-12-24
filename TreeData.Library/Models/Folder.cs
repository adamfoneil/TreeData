using AO.Models;
using AO.Models.Interfaces;
using Dapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TreeData.Library.Queries;

namespace TreeData.Library.Models
{
    [Identity(nameof(Id))]
    public class Folder : IGetRelated
    {
        public int Id { get; set; }

        [Key]
        public int ParentId { get; set; }

        [Key]
        [MaxLength(100)]
        public string Name { get; set; }

        [NotMapped]
        public bool HasChildren { get; set; }

        [NotMapped]
        public string FullName { get; set; }

        [NotMapped]
        public IEnumerable<GetPathResult> Paths { get; private set; }

        public async Task GetRelatedAsync(IDbConnection connection, IDbTransaction txn = null)
        {
            Paths = await new GetPath() { FolderId = Id }.ExecuteAsync(connection);
            FullName = string.Join(" / ", Paths.Select(row => row.Name));
            // todo: set HasChildren
        }
    }
}
