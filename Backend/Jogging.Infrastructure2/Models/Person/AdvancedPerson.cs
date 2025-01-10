using Jogging.Infrastructure2.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jogging.Infrastructure.Models.DatabaseModels.Person;

[Table("Person")]
public class AdvancedPerson
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Column("LastName")]
    public string LastName { get; set; }

    [Required]
    [Column("FirstName")]
    public string FirstName { get; set; }

    [Required]
    [Column("BirthDate")]
    public DateTime BirthDate { get; set; }

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
    public char Gender { get; set; }

    [ForeignKey("AddressId")]
    public virtual AddressEF Address { get; set; }
}