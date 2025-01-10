using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Jogging.Infrastructure2.Models;

[Table("Registration")]
public partial class RegistrationEF
{
    [Key]
    public int Id { get; set; }

    public short? RunNumber { get; set; }

    [Column(TypeName = "text")]
    public string? RunTime { get; set; }

    public bool? Paid { get; set; }

    public int CompetitionPerCategoryId { get; set; }

    public int PersonId { get; set; }

    public virtual PersonEF PersonEF { get; set; }

    public int CompetitionId { get; set; }

    public virtual CompetitionEF CompetitionEF { get; set; }
}
