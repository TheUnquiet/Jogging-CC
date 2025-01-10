using Jogging.Infrastructure.Models.DatabaseModels.CompetitionPerCategory;
using Jogging.Infrastructure.Models.DatabaseModels.Person;
using Postgrest.Attributes;
using Postgrest.Models;

namespace Jogging.Infrastructure.Models.DatabaseModels.CompetitionResult;

[Table("Registration")]
public class ExtendedCompetitionResult : BaseModel
{
    [PrimaryKey]
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

    [Reference(typeof(CompetitionResultCompetitionPerCategory), ReferenceAttribute.JoinType.Inner)]
    public CompetitionResultCompetitionPerCategory CompetitionPerCategory { get; set; }

    [Reference(typeof(AdvancedPerson), ReferenceAttribute.JoinType.Left)]
    public AdvancedPerson Person { get; set; }
}