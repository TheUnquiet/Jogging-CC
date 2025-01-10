using System.ComponentModel.DataAnnotations;
using Postgrest.Attributes;
using Postgrest.Models;

namespace Jogging.Infrastructure.Models.DatabaseModels.Club {
    [Table("Club")]
    public class SimpleClub : BaseModel {
        [PrimaryKey]
        public int Id { get; set; }

        [Required]
        [Column("Name")]
        public string Name { get; set; } 

        [Column("Logo")]
        public string? Logo { get; set; } 
    }
}
