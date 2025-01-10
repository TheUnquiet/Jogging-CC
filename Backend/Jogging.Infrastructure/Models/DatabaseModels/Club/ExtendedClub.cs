using System.ComponentModel.DataAnnotations;
using Postgrest.Attributes;
using Postgrest.Models;
using Jogging.Infrastructure.Models.DatabaseModels.Person; 

namespace Jogging.Infrastructure.Models.DatabaseModels.Club {
    [Table("Club")]
    public class ExtendedClub : BaseModel {
        [PrimaryKey]
        public int Id { get; set; }

        [Required]
        [Column("Name")]
        public string Name { get; set; } 

        [Column("Logo")]
        public string? Logo { get; set; }

        [Reference(typeof(ExtendedPerson), ReferenceAttribute.JoinType.Left)]
        public List<ExtendedPerson>? Members { get; set; }  
    }
}
