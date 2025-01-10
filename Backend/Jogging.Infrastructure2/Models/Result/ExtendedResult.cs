using Jogging.Infrastructure2.Models;
using Jogging.Infrastructure2.Models.CompetitionPerCategory;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jogging.Infrastructure.Models.DatabaseModels.Result;

[Table("Registration")]
public class ExtendedResult
{
    [Key]
    public int Id { get; set; }

    [Column("RunNumber")]
    public short? RunNumber { get; set; }

    [Column("RunTime")]
    public TimeSpan? RunTime { get; set; }

    [Column("CompetitionPerCategoryId")]
    public int CompetitionPerCategoryId { get; set; }
    
    [Column("Paid")]
    public bool? Paid { get; set; }
    
    [Column("CompetitionId")]
    public int CompetitionId { get; set; }
    
    [Column("PersonId")]
    public int PersonId { get; set; }

    [ForeignKey("CompetitionPerCategoryId")]
    public virtual CompetitionResultCompetitionPerCategory CompetitionPerCategory { get; set; }

    [ForeignKey("CompetitionId")]
    public virtual CompetitionEF Competition { get; set; }
}