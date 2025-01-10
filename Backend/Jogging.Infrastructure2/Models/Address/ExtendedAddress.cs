using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jogging.Infrastructure2.Models.DatabaseModels.Address;

[Table("Address")]
public class ExtendedAddress
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("HouseNumber")]
    public string? HouseNumber { get; set; }

    [Column("Street")]
    public string? Street { get; set; }

    [Column("ZipCode")]
    public string? ZipCode { get; set; }

    [Required]
    [Column("City")]
    public string City { get; set; } = null!;

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + (Street ?? "").GetHashCode();
            hash = hash * 23 + (HouseNumber ?? "").GetHashCode();
            hash = hash * 23 + (ZipCode ?? "").GetHashCode();
            hash = hash * 23 + (City ?? "").GetHashCode();
            return hash;
        }
    }

    public override bool Equals(object? obj)
    {
        if (obj is not ExtendedAddress other)
            return false;
        return Street == other.Street &&
               HouseNumber == other.HouseNumber &&
               ZipCode == other.ZipCode &&
               City == other.City;
    }
}