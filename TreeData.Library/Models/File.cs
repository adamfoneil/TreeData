using AO.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace TreeData.Library.Models
{
    [Identity(nameof(Id))]
    public class File
    {
        public int Id { get; set; }

        [Key]
        [References(typeof(Folder))]
        public int FolderId { get; set; }

        [Key]
        [MaxLength(100)]        
        public string Name { get; set; }

        public long Length { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }
    }
}
