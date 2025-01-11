using AutoMapper;
using Jogging.Domain.Exceptions;
using Jogging.Domain.Helpers;
using Jogging.Domain.Interfaces.RepositoryInterfaces;
using Jogging.Domain.Models;
using Jogging.Domain.Validators;

namespace Jogging.Domain.DomainManagers
{
    public class RegistrationManager
    {
        private readonly IRegistrationRepo _registrationRepo;
        private readonly IPersonRepo _personRepo;
        private readonly AuthManager _authManager;
        private readonly EmailManager _emailManager;

        public RegistrationManager(IRegistrationRepo registrationRepo, IPersonRepo personRepo, AuthManager authManager, EmailManager emailManager)
        {
            _registrationRepo = registrationRepo;
            _personRepo = personRepo;
            _authManager = authManager;
            _emailManager = emailManager;
        }

        public async Task<PagedList<RegistrationDom>> GetPersonRegistrations(int personId, QueryStringParameters parameters, bool withRunNumber)
        {
            var personRegistrations = await _registrationRepo.GetByPersonIdAsync(personId, withRunNumber);

            if (!personRegistrations.Any())
            {
                throw new RegistrationNotFoundException("No registrations found");
            }

            return PagedList<RegistrationDom>.ToPagedList(personRegistrations.AsQueryable(), parameters.PageNumber, parameters.PageSize);
        }

        public async Task<PagedList<RegistrationDom>> GetRegistrationsAsync(int? personId, int? competitionId, string? searchValue,
            bool withRunNumber, QueryStringParameters parameters)
        {
            IEnumerable<RegistrationDom> registrations;

            if (personId.HasValue && competitionId.HasValue)
            {
                registrations =
                    await _registrationRepo.GetByPersonIdAndCompetitionIdAsync(personId.Value, competitionId.Value, withRunNumber);
            }
            else if (personId.HasValue)
            {
                registrations = await _registrationRepo.GetByPersonIdAsync(personId.Value, withRunNumber);
            }
            else if (competitionId.HasValue)
            {
                if (searchValue == null)
                {
                    registrations = await _registrationRepo.GetByCompetitionIdAsync(competitionId.Value, withRunNumber);
                }
                else
                {
                    registrations =
                        await _registrationRepo.GetByCompetitionIdAndSearchValueAsync(competitionId.Value, searchValue, withRunNumber);
                }
            }
            else
            {
                if (withRunNumber)
                {
                    registrations = await _registrationRepo.GetAllAsync(withRunNumber);
                }
                else
                {
                    registrations = await _registrationRepo.GetAllAsync();
                }
            }

            if (!registrations.Any())
            {
                throw new RegistrationNotFoundException("No registrations found");
            }

            return PagedList<RegistrationDom>.ToPagedList(registrations.AsQueryable(), parameters.PageNumber, parameters.PageSize);
        }

        public async Task DeleteUserRegistration(int personId, int competitionId)
        {
            await _registrationRepo.DeleteByPersonIdAndCompetitionIdAsync(personId, competitionId);
        }

        public async Task<RegistrationDom> SignInExistingPersonToContestAsync(int competitionId, int personId, string distanceName)
        {
            var person = await _personRepo.GetByIdAsync(personId);
            return await _registrationRepo.SignInToContestAsync(competitionId, person, distanceName);
        }

        public async Task<RegistrationDom> SignInNewPersonToContestAsync(int competitionId, PersonDom personDom, string distanceName)
        {
            PersonValidator.ValidatePersonRequest(personDom);

            var newPerson = await _personRepo.AddAsync(personDom);
            return await _registrationRepo.SignInToContestAsync(competitionId, newPerson, distanceName);
        }

        public async Task<RegistrationDom> SignInToContestWithEmailAsync(int competitionId, PersonDom personDom, string email, string distanceName)
        {
            PersonValidator.ValidatePersonRequest(personDom);
            AuthenticationValidator.ValidateEmailInput(email);

            await _authManager.CheckDuplicateEmailAddressAsync(email);
            PersonDom newPerson = await _authManager.RegisterUserAsync(email, null, personDom, false);
            var registration = await _registrationRepo.SignInToContestAsync(competitionId, newPerson, distanceName);
            await _authManager.ResetUserConfirmToken(email);
            string passwordResetToken = await _authManager.ResetUserPasswordToken(email);

            _emailManager.SendRegistrationConfirmationEmail(email, passwordResetToken, $"{newPerson.FirstName} {newPerson.LastName}", registration.Competition.Name);

            return registration;
        }

        public async Task UpdatePaidAsync(int registrationId, RegistrationDom registrationRequestDom)
        {
            await _registrationRepo.UpdatePaidAsync(registrationId, registrationRequestDom);
        }
        public async Task UpdateRunNumberAsync(int registrationId, RegistrationDom registrationRequestDom)
        {
            await _registrationRepo.UpdateRunNumberAsync(registrationId, registrationRequestDom);
        }        
        public async Task UpdateCompetitionPerCategoryAsync(int registrationId, int personId, CompetitionPerCategoryDom competitionPerCategoryDom)
        {
            await _registrationRepo.UpdateCompetitionPerCategoryAsync(registrationId, personId, competitionPerCategoryDom);
        }

        public async Task<RegistrationDom> GetRegistrationAsync(int personId, int competitionId)
        {
            var registration = await _registrationRepo.GetRegistrationByPersonIdAndCompetitionIdAsync(personId, competitionId);

            if (registration == null)
            {
                throw new RegistrationNotFoundException("No registrations found");
            }

            return registration;
        }

        public async Task DeleteUserRegistration(int registrationId)
        {
            await _registrationRepo.DeleteAsync(registrationId);
        }
    }
}