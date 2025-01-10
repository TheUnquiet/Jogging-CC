using System.ComponentModel.DataAnnotations;
using Jogging.Infrastructure.Models.DatabaseModels.Account;
using Jogging.Infrastructure.Models.DatabaseModels.Address;
using Jogging.Infrastructure.Models.DatabaseModels.School;
using Jogging.Infrastructure.Models.DatabaseModels.Club;
using Postgrest.Attributes;
using Postgrest.Models;

namespace Jogging.Infrastructure.Models.DatabaseModels.Person
{
    [Table("Person")]
    public class ExtendedPerson : BaseModel
    {
        [PrimaryKey]
        public int Id { get; set; }

        [Required]
        [Column("LastName")]
        public string LastName { get; set; }

        [Required]
        [Column("FirstName")]
        public string FirstName { get; set; }

        [Required]
        [Column("BirthDate")]
        public DateOnly BirthDate { get; set; }

        [Column("IBANNumber")]
        public string IBANNumber { get; set; }

        [Column("SchoolId")]
        public int? SchoolId { get; set; }

        [Column("AddressId")]
        public int? AddressId { get; set; }

        [Column("UserId")]
        public string? UserId { get; set; }
    
        [Column("Email")]
        public string? Email { get; set; }
        
        [Required]
        [Column("Gender")]
        public Char Gender { get; set; }

        [Column("LoopClubId")]
        public int? LoopClubId { get; set; }

        [Reference(typeof(SimpleAddress), ReferenceAttribute.JoinType.Inner)]
        public SimpleAddress Address { get; set; }
        
        [Reference(typeof(Profile), ReferenceAttribute.JoinType.Left)]
        public Profile? Profile { get; set; }
        
        [Reference(typeof(SimpleSchool), ReferenceAttribute.JoinType.Left)]
        public SimpleSchool? School { get; set; }

        [Reference(typeof(SimpleClub), ReferenceAttribute.JoinType.Left)]
        public SimpleClub? Club { get; set; }
    }
}