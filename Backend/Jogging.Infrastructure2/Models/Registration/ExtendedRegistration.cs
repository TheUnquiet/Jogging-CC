using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jogging.Infrastructure2.Models.CompetitionPerCategory;
using Jogging.Infrastructure.Models.DatabaseModels.Person;

namespace Jogging.Infrastructure2.Models.Registration
{
    public class ExtendedRegistration
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

        [ForeignKey("PersonId")]
        public virtual AdvancedPerson Person { get; set; }

        [ForeignKey("CompetitionPerCategoryId")]
        public virtual CompetitionResultCompetitionPerCategory CompetitionPerCategory { get; set; }
    }
}
