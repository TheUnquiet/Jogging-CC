using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Jogging.Infrastructure2.Models;

[Table("Address")]
public partial class AddressEF
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string? Street { get; set; }

    [StringLength(100)]
    public string City { get; set; } = null!;

    [StringLength(50)]
    public string? HouseNumber { get; set; }

    [StringLength(20)]
    public string? ZipCode { get; set; }
}
