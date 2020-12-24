using AO.Models;
using AO.Models.Interfaces;
using Dapper;
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

        public async Task GetRelatedAsync(IDbConnection connection, IDbTransaction txn = null)
        {
            FullName = await GetFullNameAsync(connection, Id);
            // todo: set HasChildren
        }

        private static async Task<string> GetFullNameAsync(IDbConnection connection, int id)
        {
            var folders = await new GetPath() { FolderId = id }.ExecuteAsync(connection);
            return string.Join(" / ", folders.Select(row => row.Name));
        }
    }
}
