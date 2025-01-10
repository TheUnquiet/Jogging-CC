using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Jogging.Infrastructure2.Models;

[Table("CompetitionPerCategory")]
public partial class CompetitionPerCategoryEF
{
    [Key]
    public int Id { get; set; }

    [StringLength(1)]
    public string? Gender { get; set; }

    [StringLength(30)]
    public string? DistanceName { get; set; }

    public float? DistanceInKm { get; set; }

    public int? AgeCategoryId { get; set; }

    public int? CompetitionId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? GunTime { get; set; }
}
