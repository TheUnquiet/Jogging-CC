using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Jogging.Infrastructure2.Models;

[Table("Competition")]
public partial class CompetitionEF
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? Date { get; set; }

    public int? AddressId { get; set; }

    public bool? Active { get; set; }

    [Column("img_url", TypeName = "text")]
    public string? ImgUrl { get; set; }

    [Column(TypeName = "text")]
    public string? Information { get; set; }

    [Column("url", TypeName = "text")]
    public string? Url { get; set; }

    public bool? RankingActive { get; set; }

}
