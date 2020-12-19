using AO.Models;
using System.ComponentModel.DataAnnotations;

namespace TreeData.Library.Models
{
    [Identity(nameof(Id))]
    public class Folder
    {
        public int Id { get; set; }

        [Key]
        public int ParentId { get; set; }

        [Key]
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
