using Jogging.Domain.Helpers;
using Jogging.Domain.Interfaces.RepositoryInterfaces;
using Jogging.Domain.Models;
using Jogging.Domain.Validators;

namespace Jogging.Domain.DomainManagers;

public class AuthManager {
    private readonly IAuthenticationRepo _authRepo;
    private readonly EmailManager _emailManager;
    private readonly PersonManager _personManager;
    private readonly CustomMemoryCache _memoryCache;

    public AuthManager(IAuthenticationRepo authRepo, EmailManager emailManager, PersonManager personManager, CustomMemoryCache memoryCache) {
        _authRepo = authRepo;
        _emailManager = emailManager;
        _personManager = personManager;
        _memoryCache = memoryCache;
    }

    public async Task<PersonDom> SignInAsync(string email, string password) {
        AuthenticationValidator.ValidateEmailInput(email);

        var loggedInPersonDom = await _authRepo.SignInAsync(email, password);

        return loggedInPersonDom;
    }

    public async Task<string> SignUpAsync(string email, string password, PersonDom personDom, bool sendConfirmEmail) {
        AuthenticationValidator.ValidateEmailInput(email);

        if (string.IsNullOrEmpty(password) || password.Length < 6) {
            throw new ArgumentException("Password must be at least 6 characters long.");
        }

        await CheckDuplicateEmailAddressAsync(email);

        return await _authRepo.SignUpAsync(personDom, password);
    }

    public async Task<PersonDom> RegisterUserAsync(string email, string password, PersonDom signedUpPersonDom, bool sendConfirmEmail) {
        if (string.IsNullOrEmpty(password)) {
            throw new ArgumentException("Password cannot be empty.");
        }

        var userId = await SignUpAsync(email, password, signedUpPersonDom, sendConfirmEmail);

        signedUpPersonDom.Email = email;
        signedUpPersonDom.UserId = userId;

        AddressValidator.ValidateCity(signedUpPersonDom.Address.City);
        PersonValidator.ValidatePersonRequest(signedUpPersonDom);

        var person = await _personManager.CreatePersonAsync(signedUpPersonDom);

        if (sendConfirmEmail) {
            string confirmToken = await ResetUserConfirmToken(email);
            _emailManager.SendConfirmEmail(email, confirmToken, person);
        }

        return person;
    }

    public async Task ChangePassword(PasswordChangeDom passwordChange) {
        PasswordValidator.ValidatePassword(passwordChange.newPassword);

        await _authRepo.ChangePassword(passwordChange);
    }

    public async Task CheckDuplicateEmailAddressAsync(string email) {
        AuthenticationValidator.ValidateEmailInput(email);

        await _authRepo.CheckDuplicateEmailAddressAsync(email);
    }

    public async Task ResetPasswordAsync(PasswordResetDom passwordResetDom) {
        PasswordValidator.ValidatePassword(passwordResetDom.newPassword);
        await _authRepo.ResetPassword(passwordResetDom);
    }

    public async Task<string> ResetUserConfirmToken(string email) {
        return await _authRepo.ResetUserConfirmToken(email);
    }

    public async Task<string> ResetUserPasswordToken(string email) {
        return await _authRepo.ResetUserPasswordToken(email);
    }

    public async Task ConfirmEmailAsync(ConfirmTokenDom confirmTokenDom) {
        await _authRepo.ConfirmEmail(confirmTokenDom);
    }

    public async Task<PersonDom> UpdatePersonEmailAsync(int personId, PersonDom updatedPerson) {
        var currentPerson = await _personManager.GetByIdAsync(personId);
        string? oldEmail = currentPerson.Email;
        string? newEmail = updatedPerson.Email;

        if (string.IsNullOrWhiteSpace(oldEmail) && !string.IsNullOrWhiteSpace(newEmail)) {
            string userId = await SignUpAsync(newEmail, "defaultPassword123", updatedPerson, true);

            currentPerson.Email = newEmail;
            currentPerson.UserId = userId;

            await _personManager.UpdatePersonEmailAsync(personId, currentPerson);

            string resetUserPasswordToken = await ResetUserPasswordToken(newEmail);
            _emailManager.SendRegistrationConfirmationEmail(newEmail, resetUserPasswordToken, $"{currentPerson.FirstName} {currentPerson.LastName}", null);
        } else if (!string.IsNullOrWhiteSpace(oldEmail) && string.IsNullOrWhiteSpace(newEmail)) {
            currentPerson.Email = null;
            currentPerson.UserId = null;

            await _personManager.UpdatePersonEmailAsync(personId, currentPerson);
            await _authRepo.RemoveUserEmailAsync(oldEmail);
        } else if (!string.IsNullOrWhiteSpace(oldEmail) && !string.IsNullOrWhiteSpace(newEmail) && oldEmail != newEmail) {
            AuthenticationValidator.ValidateEmailInput(newEmail);

            await CheckDuplicateEmailAddressAsync(newEmail);

            currentPerson.Email = newEmail;
            await _authRepo.UpdateUserEmail(oldEmail, newEmail);
            await _personManager.UpdatePersonEmailAsync(personId, currentPerson);
            string confirmToken = await ResetUserConfirmToken(newEmail);
            _emailManager.SendEmailChangedMail(newEmail, confirmToken, currentPerson);
        }

        _memoryCache.RemoveKeysWithPattern(@"^results_runtime_");
        _memoryCache.Remove(CacheKeyGenerator.GetAllResultsKey());

        return currentPerson;
    }
}