using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Jogging.Infrastructure2.Views;

[Keyless]
public partial class Personview
{
    public int Id { get; set; }

    [StringLength(50)]
    public string LastName { get; set; } = null!;

    [StringLength(50)]
    public string FirstName { get; set; } = null!;

    public DateOnly BirthDate { get; set; }

    [Column("IBANNumber")]
    [StringLength(30)]
    public string? Ibannumber { get; set; }

    public int? SchoolId { get; set; }

    public int? AddressId { get; set; }

    public Guid? UserId { get; set; }

    [StringLength(10)]
    public string? Gender { get; set; }

    [StringLength(255)]
    public string? Email { get; set; }
}
