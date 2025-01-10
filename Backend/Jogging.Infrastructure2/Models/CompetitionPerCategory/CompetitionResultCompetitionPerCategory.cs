using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jogging.Infrastructure2.Models.CompetitionPerCategory
{
    [Table("CompetitionPerCategory")]
    public class CompetitionResultCompetitionPerCategory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("DistanceName")]
        public string DistanceName { get; set; }

        [Column("DistanceInKm")]
        public float DistanceInKm { get; set; }

        [Column("Gender")]
        public string Gender { get; set; }

        [Column("AgeCategoryId")]
        public int AgeCategoryId { get; set; }

        [Column("CompetitionId")]
        public int CompetitionId { get; set; }

        [Column("GunTime")]
        public DateTime? GunTime { get; set; }

        [ForeignKey("AgeCategoryId")]
        public virtual AgeCategoryEF AgeCategory { get; set; }
    }
}
