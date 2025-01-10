using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Jogging.Domain.Models
{
    public class PersonDom
    {
        [Key]
        public int Id { get; set; }

        private string _firstName;
        private string _lastName;

        [Required]
        [StringLength(50)]
        public string FirstName
        {
            get => _firstName;
            set => _firstName = value.Trim();
        }

        [Required]
        [StringLength(50)]
        public string LastName
        {
            get => _lastName;
            set => _lastName = value.Trim();
        }

        [Required]
        public DateOnly BirthDate { get; set; }

        [StringLength(30)]
        public string IBANNumber { get; set; }

        public char Gender { get; set; }
        
        public string? UserId { get; set; }
        public int? SchoolId { get; set; }
        public SchoolDom? School { get; set; }

        public int? AddressId { get; set; }
        public AddressDom Address { get; set; }

        public ProfileDom? Profile { get; set; }
        
        public string? Email { get; set; }

        public int? ClubId { get; set; }
        public ClubDom? Club { get; set; }

        [JsonIgnore]
        public int Age
        {
            get
            {
                var timeDifference = (DateTime.Now.Date - BirthDate.ToDateTime(TimeOnly.MinValue).Date);
                return (int)Math.Floor(timeDifference.Days / 365m);
            }
        }

        [JsonIgnore]
        public int BirthYearAge
        {
            get
            {
                var currentYear = DateTime.Now.Year;
                var personBirthYear = BirthDate.Year;
                return currentYear - personBirthYear;
            }
        }
        
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + (FirstName ?? "").GetHashCode();
                hash = hash * 23 + (LastName ?? "").GetHashCode();
                hash = hash * 23 + (BirthDate).GetHashCode();
                hash = hash * 23 + (IBANNumber ?? "").GetHashCode();
                hash = hash * 23 + (Gender).GetHashCode();
                hash = hash * 23 + (UserId ?? "").GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is PersonDom other))
                return false;

            return FirstName == other.FirstName && LastName == other.LastName && BirthDate == other.BirthDate && IBANNumber == other.IBANNumber &&
                   Gender == other.Gender;
        }
    }
}
