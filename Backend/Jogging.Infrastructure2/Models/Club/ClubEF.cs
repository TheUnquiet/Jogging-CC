using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Jogging.Infrastructure2.Models;

namespace Jogging.Infrastructure2.Models.Club;

[Table("Club")]
public partial class ClubEF
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(255)]
    public string? Logo { get; set; }

    public ICollection<PersonEF>? Members { get; set; }
}
