using AO.Models;
using System.ComponentModel.DataAnnotations;

namespace TreeData.Library.Models
{
    [Identity(nameof(Id))]
    public class IgnoreFolderName
    {
        public int Id { get; set; }

        [MaxLength(50)]
        [Key]
        public string Name { get; set; }
    }
}

/*
ModelSync not working with netstandard2.1

CREATE TABLE [dbo].[IgnoreFolder] (
	[Id] int identity(1,1),
	[Name] nvarchar(50) NOT NULL,
	CONSTRAINT [U_IgnoreFolder_Id] UNIQUE ([Id]),
	CONSTRAINT [PK_IgnoreFolder] PRIMARY KEY ([Name])
)
*/