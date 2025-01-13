using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Jogging.Domain.Exceptions;
using Jogging.Domain.Helpers;
using Jogging.Domain.Interfaces.RepositoryInterfaces;
using Jogging.Domain.Models;
using Jogging.Infrastructure2.Data;
using Jogging.Infrastructure2.Models;
using Jogging.Infrastructure2.Models.Account;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace Jogging.Infrastructure2.Repositories.MySqlRepositories {
    public class AuthRepo : IAuthenticationRepo {
        private readonly JoggingCcContext _dbContext;
        private readonly IMapper _mapper;

        public AuthRepo(JoggingCcContext dbContext, IMapper mapper) {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<PersonDom> SignInAsync(string email, string password) {
            PersonEF personEF = await _dbContext.People
                .AsNoTracking()
                .Include(p => p.Profile)
                .FirstOrDefaultAsync(p => p.Email.ToLower() == email.ToLower());

            if (personEF == null) {
                throw new AuthException("The given user information was incorrect.");
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, personEF.PasswordHash);
            if (!isPasswordValid) {
                throw new AuthException("The given user information was incorrect.");
            }

            PersonDom personDom = new PersonDom {
                Id = personEF.Id,
                FirstName = personEF.FirstName,
                LastName = personEF.LastName,
                Email = personEF.Email,
                Password = password,
                BirthDate = personEF.BirthDate,
                Gender = !string.IsNullOrEmpty(personEF.Gender) ? personEF.Gender[0] : '\0',
                IBANNumber = personEF.Ibannumber,
                SchoolId = personEF.SchoolId,
                AddressId = personEF.AddressId,
                ClubId = personEF.ClubId,
                UserId = personEF.UserId?.ToString(),
                Profile = personEF.Profile != null ? new ProfileDom {
                    Id = personEF.Profile.Id.ToString(),
                    Role = personEF.Profile.Role,
                    PersonId = personEF.Profile.PersonId
                } : null,
            };

            return personDom;
        }

        public async Task<string> SignUpAsync(PersonDom personDom, string password) {
            if (string.IsNullOrEmpty(password)) {
                password = Guid.NewGuid().ToString().Substring(0, 8);
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var personEF = new PersonEF {
                FirstName = personDom.FirstName,
                LastName = personDom.LastName,
                BirthDate = personDom.BirthDate,
                Email = personDom.Email,
                Ibannumber = personDom.IBANNumber,
                Gender = personDom.Gender.ToString(),
                PasswordHash = passwordHash,
                SchoolId = personDom.SchoolId,
                AddressId = personDom.AddressId,
                ClubId = personDom.ClubId
            };

            _dbContext.People.Add(personEF);
            await _dbContext.SaveChangesAsync();

            var profileEF = new ProfileEF {
                Id = Guid.NewGuid(),
                Role = "Admin",
                PersonId = personEF.Id
            };

            _dbContext.Profiles.Add(profileEF);
            await _dbContext.SaveChangesAsync();

            return personEF.Id.ToString();
        }

        public async Task ChangePassword(PasswordChangeDom passwordChangeInfo) {
            var userGuid = passwordChangeInfo.UserId;

            var user = await _dbContext.People.FirstOrDefaultAsync(p => p.UserId == userGuid);
            if (user == null) {
                throw new PersonException("User not found");
            }

            if (!BCrypt.Net.BCrypt.Verify(passwordChangeInfo.oldPassword, user.PasswordHash)) {
                throw new AuthException("Old password is incorrect.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(passwordChangeInfo.newPassword);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<string> ResetUserConfirmToken(string email) {
            string confirmToken = TokenGenerator.GenerateEmailToken(email);

            var user = await _dbContext.People.FirstOrDefaultAsync(p => p.Email == email);
            if (user == null) {
                throw new PersonException("User not found");
            }

            user.ConfirmationToken = confirmToken;
            await _dbContext.SaveChangesAsync();

            return confirmToken;
        }

        public async Task ConfirmEmail(ConfirmTokenDom confirmTokenDom) {
            var user = await _dbContext.People.FirstOrDefaultAsync(p => p.ConfirmationToken == confirmTokenDom.confirm_token);
            if (user == null) {
                throw new PersonException("Invalid token");
            }

            user.IsEmailConfirmed = true;
            await _dbContext.SaveChangesAsync();
        }


        public async Task<string> ResetUserPasswordToken(string email) {
            string resetToken = TokenGenerator.GenerateEmailToken(email);

            var user = await _dbContext.People.FirstOrDefaultAsync(p => p.Email == email);
            if (user == null) {
                throw new PersonException("User not found");
            }

            user.PasswordResetToken = resetToken;
            await _dbContext.SaveChangesAsync();

            return resetToken;
        }


        public async Task CheckDuplicateEmailAddressAsync(string email) {
            var exists = await _dbContext.People.AnyAsync(p => p.Email == email);
            if (exists) {
                throw new DuplicateEmailException("This email address is already registered.");
            }
        }

        public async Task RemoveUserEmailAsync(string email) {
            var user = await _dbContext.People.FirstOrDefaultAsync(p => p.Email == email);
            if (user == null) {
                throw new PersonException("User not found");
            }

            _dbContext.People.Remove(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateUserEmail(string oldEmail, string newEmail) {
            var user = await _dbContext.People.FirstOrDefaultAsync(p => p.Email == oldEmail);
            if (user == null) {
                throw new PersonException("User not found");
            }

            user.Email = newEmail;
            await _dbContext.SaveChangesAsync();
        }

        public async Task ResetPassword(PasswordResetDom passwordReset) {
            var user = await _dbContext.People.FirstOrDefaultAsync(p => p.Email == passwordReset.email);
            if (user == null) {
                throw new PersonException("User not found");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(passwordReset.newPassword);
            await _dbContext.SaveChangesAsync();
        }
    }
}
