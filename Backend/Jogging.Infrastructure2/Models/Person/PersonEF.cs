using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Jogging.Infrastructure2.Models.Club;
using Jogging.Infrastructure2.Models.Account;

namespace Jogging.Infrastructure2.Models;

[Table("Person")]
public partial class PersonEF
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string LastName { get; set; }

    [StringLength(50)]
    public string FirstName { get; set; }

    public DateOnly BirthDate { get; set; }

    public string PasswordHash { get; set; }

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

    public int? ClubId { get; set; }

    public ClubEF? Club { get; set; }

    public virtual ProfileEF? Profile { get; set; }

    public string? ConfirmationToken { get; set; }

    public string? PasswordResetToken { get; set; }

    public bool IsEmailConfirmed { get; set; }
}
