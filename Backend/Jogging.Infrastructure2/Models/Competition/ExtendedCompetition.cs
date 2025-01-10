using Jogging.Infrastructure2.Models;
using Jogging.Infrastructure2.Models.CompetitionPerCategory;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Jogging.Infrastructure2.Models.DatabaseModels.Competition;

[Table("Competition")]
public class ExtendedCompetition
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Column("Name")]
    public string Name { get; set; }

    [Column("Date")]
    public DateTime? Date { get; set; }

    [Column("Active")]
    public bool Active { get; set; } = false;

    [Column("RankingActive")]
    public bool RankingActive { get; set; } = false;

    [Column("Information")]
    public string? Information { get; set; }

    [Column("AddressId")]
    public int? AddressId { get; set; }

    [ForeignKey("AddressId")]
    public virtual AddressEF? Address { get; set; }

    [JsonIgnore]
    public Dictionary<string, float>? Distances { get; set; }

    public virtual List<ExtendedCompetitionPerCategory>? CompetitionPerCategories { get; set; }
}