using Jogging.Domain.Models;

namespace Jogging.Domain.Interfaces.RepositoryInterfaces
{
    public interface IAuthenticationRepo
    {
        public Task<PersonDom> SignInAsync(string email, string password);
        public Task<string> SignUpAsync(PersonDom person, string password);
        public Task<string> ResetUserConfirmToken(string email);
        public Task ConfirmEmail(ConfirmTokenDom confirmTokenDom);
        public Task ChangePassword(PasswordChangeDom passwordChangeInfo);
        public Task CheckDuplicateEmailAddressAsync(string email);
        public Task RemoveUserEmailAsync(string email);
        public Task ResetPassword(PasswordResetDom map);
        public Task<string> ResetUserPasswordToken(string email);
        public Task UpdateUserEmail(string oldEmail, string newEmail);
    }
}