﻿using Jogging.Infrastructure2.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jogging.Infrastructure.Models.DatabaseModels.Registration;

[Table("Registration")]
public class PersonRegistration
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
    public virtual PersonEF Person { get; set; }

    [ForeignKey("CompetitionId")]
    public virtual CompetitionEF Competition { get; set; }
}