using AutoMapper;
using Jogging.Domain.Exceptions;
using Jogging.Domain.Helpers;
using Jogging.Domain.Interfaces.RepositoryInterfaces;
using Jogging.Domain.Models;
using Jogging.Infrastructure.Models.DatabaseModels.Registration;
using Jogging.Infrastructure2.Data;
using Jogging.Infrastructure2.Models;
using Microsoft.EntityFrameworkCore;

namespace Jogging.Infrastructure2.Repositories.MySqlRepositories {
    public class RegistrationRepo : IRegistrationRepo {
        private readonly JoggingCcContext _context;
        private readonly IAgeCategoryRepo _ageCategoryRepo;
        private readonly IPersonRepo _personRepo;
        private readonly ICompetitionPerCategoryRepo _competitionPerCategoryRepo;
        private readonly ICompetitionRepo _competitionRepo;
        private readonly IMapper _mapper;
        private readonly CustomMemoryCache _memoryCache;

        public RegistrationRepo(JoggingCcContext context, IAgeCategoryRepo ageCategoryRepo,
            ICompetitionPerCategoryRepo competitionPerCategoryRepo, IMapper mapper, CustomMemoryCache memoryCache,
            IPersonRepo personRepo) {
            _context = context;
            _ageCategoryRepo = ageCategoryRepo;
            _competitionPerCategoryRepo = competitionPerCategoryRepo;
            _mapper = mapper;
            _memoryCache = memoryCache;
            _personRepo = personRepo;
        }

        #region GetRegistrationByPersonIdAndCompetitionIdAsync
        public async Task<RegistrationDom> GetRegistrationByPersonIdAndCompetitionIdAsync(int personId, int competitionId, bool throwError = true) {
            var personRegistration = await _context.Registrations
                .FirstOrDefaultAsync(r => r.PersonId == personId && r.CompetitionId == competitionId);

            if (throwError && personRegistration == null) {
                throw new RegistrationNotFoundException("Registration not found");
            }

            return _mapper.Map<RegistrationDom>(personRegistration);
        }
        #endregion

        #region GetAllAsync
        public async Task<List<RegistrationDom>> GetAllAsync() {
            try {
                var registrations = await _context.Registrations.ToListAsync();
                return _mapper.Map<List<RegistrationDom>>(registrations);
            } catch (Exception ex) {
                throw new Exception($"GetAllAsync: {ex.Message}");
            }
        }
        #endregion

        #region GetAllAsync (withRunNumber)
        public async Task<List<RegistrationDom>> GetAllAsync(bool withRunNumber) {
            try {
                var registrations = await _context.Registrations
                    .Where(p => p.RunNumber != null).ToListAsync();

                return _mapper.Map<List<RegistrationDom>>(registrations);
            } catch (Exception ex) {
                throw new Exception($"GetAllAsync: {ex.Message}");
            }
        }
        #endregion

        #region GetRegistrationByAndCompetitionIdAndSearchValueAsync
        public async Task<List<RegistrationDom>> GetByCompetitionIdAndSearchValueAsync(int competitionId, string searchValue, bool withRunNumber) {
            var registrationsQuery = _context.Registrations
                .Where(r => r.CompetitionId == competitionId && r.RunNumber != null)
                .AsQueryable();

            if (withRunNumber) {
                registrationsQuery = registrationsQuery.Where(r => r.RunNumber != null);
            }

            var registrations = await registrationsQuery.ToListAsync();

            if (registrations == null || !registrations.Any()) {
                throw new RegistrationNotFoundException("No registrations found");
            }

            return _mapper.Map<List<RegistrationDom>>(registrations);
        }
        #endregion

        #region GetRegistrationsByPersonIdAndCompetitionIdAsync
        public async Task<List<RegistrationDom>> GetByPersonIdAndCompetitionIdAsync(int personId, int competitionId, bool withRunNumber, bool throwError = true) {
            var registrations = await _context.Registrations
                .Where(r => r.PersonId == personId && r.CompetitionId == competitionId)
                .ToListAsync();

            if (throwError && !registrations.Any()) {
                throw new RegistrationNotFoundException("No registrations found");
            }

            return _mapper.Map<List<RegistrationDom>>(registrations);
        }
        #endregion

        #region GetRegistrationsByPersonIdAsync
        public async Task<List<RegistrationDom>> GetByPersonIdAsync(int personId, bool withRunNumber) {
            var registrations = await _context.Registrations
                .Where(r => r.PersonId == personId)
                .ToListAsync();

            if (registrations == null || !registrations.Any()) {
                throw new RegistrationNotFoundException("No registrations found");
            }

            return _mapper.Map<List<RegistrationDom>>(registrations);
        }
        #endregion

        #region GetRegistrationsByCompetitionIdAsync
        public async Task<List<RegistrationDom>> GetByCompetitionIdAsync(int competitionId, bool withRunNumber) {
            var registrations = await _context.Registrations
                .Where(r => r.CompetitionId == competitionId)
                .ToListAsync();

            if (registrations == null || !registrations.Any()) {
                throw new RegistrationNotFoundException("No registrations found");
            }

            return _mapper.Map<List<RegistrationDom>>(registrations);
        }
        #endregion

        #region DeleteByPersonIdAndCompetitionIdAsync
        public async Task DeleteByPersonIdAndCompetitionIdAsync(int personId, int competitionId) {
            try {
                var registration = await _context.Registrations
                    .FirstOrDefaultAsync(r => r.PersonId == personId && r.CompetitionId == competitionId)
                    ?? throw new Exception($"Registration not found for PersonId {personId} and CompetitionId {competitionId}");

                _context.Registrations.Remove(registration);
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                throw new Exception($"DeleteByPersonIdAndCompetitionIdAsync: {ex.Message}");
            }
        }
        #endregion

        #region DeleteAsync
        public async Task DeleteAsync(int registrationId) {
            var registration = await _context.Registrations.FindAsync(registrationId)
                ?? throw new Exception($"Registration with ID {registrationId} not found");

            _context.Registrations.Remove(registration);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region GetByIdAsync
        public async Task<RegistrationDom> GetByIdAsync(int registrationId) {
            var registration = await _context.Registrations
                .FirstOrDefaultAsync(r => r.Id == registrationId);

            if (registration == null) {
                throw new RegistrationNotFoundException("Registration not found");
            }

            return _mapper.Map<RegistrationDom>(registration);
        }
        #endregion

        #region SignInToContestAsync
        public async Task<RegistrationDom> SignInToContestAsync(int competitionId, PersonDom person, string distanceName) {
            await CheckDuplicateRegistration(person.Id, competitionId);

            var competition = await _competitionRepo.GetSimpleCompetitionByIdAsync(competitionId);
            var ageCategory = await _ageCategoryRepo.GetAgeCategoryByAge(person);
            var competitionPerCategory = await _competitionPerCategoryRepo
                .GetCompetitionPerCategoryByParameters(ageCategory.Id, distanceName, person.Gender, competitionId);

            var registrationEF = new RegistrationEF {
                CompetitionId = competitionId,
                PersonId = person.Id,
                Paid = false,
                CompetitionPerCategoryId = competitionPerCategory.Id
            };

            _context.Registrations.Add(registrationEF);
            await _context.SaveChangesAsync();

            if (registrationEF.Id == 0) {
                throw new PersonRegistrationException("Something went wrong during registration");
            }

            var registrationDom = _mapper.Map<RegistrationDom>(registrationEF);

            return registrationDom;
        }
        #endregion

        #region UpdateAsync
        public async Task<RegistrationDom> UpdateAsync(int registrationId, RegistrationDom updatedItem) {
            var oldRegistration = await GetSimpleRegistrationByIdAsync(registrationId);

            if (oldRegistration == null) {
                throw new RegistrationNotFoundException("Registration not found");
            }

            if (updatedItem.RunNumber != null) {
                var runNumber = updatedItem.RunNumber == -1 ? null : updatedItem.RunNumber;
                oldRegistration.RunNumber = runNumber;
            }

            if (updatedItem.Paid != null) {
                oldRegistration.Paid = (bool)updatedItem.Paid;
            }

            _context.Registrations.Update(oldRegistration);
            await _context.SaveChangesAsync();

            return _mapper.Map<RegistrationDom>(oldRegistration);
        }
        #endregion

        #region CheckDuplicateRegistration
        private async Task CheckDuplicateRegistration(int personId, int competitionId) {
            var existingRegistrations = await _context.Registrations
                .Where(pr => pr.PersonId == personId && pr.CompetitionId == competitionId)
                .ToListAsync();

            if (existingRegistrations.Any()) {
                throw new RegistrationAlreadyExistsException("Deze registratie bestaat al");
            }
        }
        #endregion

        public async Task<RegistrationDom> UpdateRunNumberAsync(int registrationId, RegistrationDom updatedItem) {
            var oldRegistration = await _context.Registrations.FindAsync(registrationId);

            if (oldRegistration == null) {
                throw new RegistrationNotFoundException("Registration not found");
            }

            var runNumber = updatedItem.RunNumber == -1 ? null : updatedItem.RunNumber;
            oldRegistration.RunNumber = runNumber;

            _context.Registrations.Update(oldRegistration);
            await _context.SaveChangesAsync();

            return _mapper.Map<RegistrationDom>(oldRegistration);
        }

        #region UpdatePaidAsync
        public async Task<RegistrationDom> UpdatePaidAsync(int registrationId, RegistrationDom updatedItem) {
            var oldRegistration = await _context.Registrations.FindAsync(registrationId);

            if (oldRegistration == null) {
                throw new RegistrationNotFoundException("Registration not found");
            }

            if (oldRegistration.Paid != updatedItem.Paid) {
                oldRegistration.Paid = updatedItem.Paid;

                _context.Registrations.Update(oldRegistration);
                await _context.SaveChangesAsync();
            }

            return _mapper.Map<RegistrationDom>(oldRegistration);
        }
        #endregion


        #region UpdateCompetitionPerCategoryAsync
        public async Task<RegistrationDom> UpdateCompetitionPerCategoryAsync(int registrationId, int personId, CompetitionPerCategoryDom competitionPerCategory) {
            var oldRegistration = await _context.Registrations
                .Include(r => r.PersonEF)
                .Include(r => r.CompetitionEF)
                .FirstOrDefaultAsync(r => r.Id == registrationId);

            if (oldRegistration == null) {
                throw new RegistrationNotFoundException("Registration not found");
            }

            if (oldRegistration.PersonId != personId) {
                throw new PersonRegistrationException("You can't change this registration");
            }

            var person = await _personRepo.GetByIdAsync(personId);
            var ageCategory = await _ageCategoryRepo.GetAgeCategoryByAge(person);

            var newCompetitionPerCategory = await _competitionPerCategoryRepo
                .GetCompetitionPerCategoryByParameters(
                    ageCategory.Id,
                    competitionPerCategory.DistanceName,
                    person.Gender,
                    oldRegistration.CompetitionId
                );

            oldRegistration.CompetitionPerCategoryId = newCompetitionPerCategory.Id;

            _context.Registrations.Update(oldRegistration);
            await _context.SaveChangesAsync();

            _memoryCache.Remove(CacheKeyGenerator.GetCompetitionResultsKey(oldRegistration.CompetitionId));
            _memoryCache.Remove(CacheKeyGenerator.GetAllResultsKey());

            return _mapper.Map<RegistrationDom>(oldRegistration);
        }
        #endregion

        #region GetSimpleRegistrationByIdAsync
        private async Task<RegistrationEF?> GetSimpleRegistrationByIdAsync(int registrationId) {
            return await _context.Registrations
                .Where(c => c.Id == registrationId).FirstOrDefaultAsync();
        }
        #endregion

        #region UpsertAsync
        public Task<RegistrationDom> UpsertAsync(int? id, RegistrationDom updatedItem) {
            throw new NotImplementedException();
        }
        #endregion

        #region AddAsync
        public Task<RegistrationDom> AddAsync(RegistrationDom person) {
            throw new NotImplementedException();
        }
        #endregion
    }
}