using Postgrest.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jogging.Infrastructure.Models.DatabaseModels.Registration;

namespace Jogging.Infrastructure2.Models.CompetitionPerCategory
{
    [Table("CompetitionPerCategory")]
    public class ExtendedCompetitionPerCategory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("DistanceName")]
        public string DistanceName { get; set; }

        [Column("DistanceInKm")]
        public float DistanceInKm { get; set; }

        [Column("Gender")]
        public char Gender { get; set; }

        [Column("AgeCategoryId")]
        public int AgeCategoryId { get; set; }

        [Column("CompetitionId")]
        public int CompetitionId { get; set; }

        [Column("GunTime")]
        public DateTime? GunTime { get; set; }

        public virtual ICollection<ExtendedRegistration> Registrations { get; set; }
    }
}
