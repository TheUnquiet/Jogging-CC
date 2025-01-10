using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Jogging.Infrastructure2.Models.Account;

[Table("Profile")]
public partial class ProfileEF
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("role")]
    [StringLength(255)]
    public string? Role { get; set; }
}
