using Jogging.Infrastructure.Models.DatabaseModels.Person;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jogging.Infrastructure.Models.DatabaseModels.School;

[Table("School")]
public class ExtendedSchool
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Column("Name")]
    public string Name { get; set; }

    [InverseProperty("School")]
    public List<ExtendedPerson>? People { get; set; }
    
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + (Name ?? "").GetHashCode();
            return hash;
        }
    }

    public override bool Equals(object? obj)
    {
        if (!(obj is ExtendedSchool other))
            return false;

        return Name == other.Name;
    }
}