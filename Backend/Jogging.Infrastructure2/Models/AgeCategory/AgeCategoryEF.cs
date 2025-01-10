using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Jogging.Infrastructure2.Models;

[Table("AgeCategory")]
public partial class AgeCategoryEF
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    public int? MinimumAge { get; set; }

    public int? MaximumAge { get; set; }
}
