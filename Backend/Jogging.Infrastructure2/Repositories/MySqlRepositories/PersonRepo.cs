using AutoMapper;
using Jogging.Domain.Exceptions;
using Jogging.Domain.Helpers;
using Jogging.Domain.Interfaces.RepositoryInterfaces;
using Jogging.Domain.Models;
using Jogging.Infrastructure2.Data;
using Jogging.Infrastructure2.Models;
using Jogging.Infrastructure2.Models.DatabaseModels.Address;
using Jogging.Infrastructure2.Models.DatabaseModels.Person;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;

namespace Jogging.Infrastructure2.Repositories.MySqlRepositories {
    public class PersonRepo : IPersonRepo {
        private readonly JoggingCcContext _context;
        private readonly IGenericRepo<AddressDom> _addressRepo;
        private readonly IGenericRepo<SchoolDom> _schoolRepo;
        private readonly IMapper _mapper;

        public PersonRepo(JoggingCcContext context, IGenericRepo<AddressDom> addressRepo, IGenericRepo<SchoolDom> schoolRepo, IMapper mapper) {
            _context = context;
            _addressRepo = addressRepo;
            _schoolRepo = schoolRepo;
            _mapper = mapper;
        }

        public async Task<List<PersonDom>> GetAllAsync() {
            try {
                return _mapper.Map<List<PersonDom>>(await _context.People.ToListAsync());
            } catch (Exception ex) {
                throw new Exception($"GetAllAsync: {ex.Message}");
            }
        }

        public async Task<List<PersonDom>> GetBySearchValueAsync(string searchValue) {
            try {
                var people = await _context.People
                    .Where(p =>
                            p.FirstName.Contains(searchValue) ||
                            p.LastName.Contains(searchValue) ||
                            p.Email.Contains(searchValue))
                .ToListAsync();

                return _mapper.Map<List<PersonDom>>(people);
            } catch (Exception ex) {
                throw new Exception($"GetBySearchValueAsync: {ex.Message}");
            }
        }

        public async Task<PersonDom> GetByIdAsync(int personId) {
            try {
                return _mapper.Map<PersonDom>(await _context.People.FindAsync(personId));
            } catch (Exception ex) {
                throw new Exception($"GetByIdAsync: {ex.Message}");
            }
        }

        public async Task<PersonDom> GetByEmailAsync(string email) {
            try {
                return _mapper.Map<PersonDom>(await _context.People.FirstOrDefaultAsync(p => p.Email == email));
            } catch (Exception ex) {
                throw new Exception($"GetByEmailAsync: {ex.Message}");
            }
        }

        Task<PersonDom> IGenericRepo<PersonDom>.UpdateAsync(int id, PersonDom updatedItem) {
            throw new NotImplementedException();
        }

        public async Task<PersonDom> AddAsync(PersonDom newPersonDom) {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try {
                if (newPersonDom == null) {
                    throw new ArgumentNullException(nameof(newPersonDom), "The input cannot be null.");
                }

                var address = await _context.Addresses
                    .FirstOrDefaultAsync(a =>
                        a.Street == newPersonDom.Address.Street &&
                        a.City == newPersonDom.Address.City)
                    ?? _context.Addresses.Add(_mapper.Map<AddressEF>(newPersonDom.Address)).Entity;

                SchoolDom? school = null;

                if (newPersonDom.School != null) {
                    var existingSchool = await _context.Schools
                        .FirstOrDefaultAsync(s => s.Name == newPersonDom.School.Name);

                    if (existingSchool != null) {
                        school = _mapper.Map<SchoolDom>(existingSchool);
                    } else {
                        var newSchoolEntity = _mapper.Map<SchoolEF>(newPersonDom.School);
                        var addedSchool = _context.Schools.Add(newSchoolEntity).Entity;

                        school = _mapper.Map<SchoolDom>(addedSchool);
                    }
                }

                var person = _mapper.Map<PersonEF>(newPersonDom);
                person.AddressId = address.Id;
                person.SchoolId = school?.Id;

                _context.People.Add(person);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return _mapper.Map<PersonDom>(person);
            } catch (Exception ex) {
                await transaction.RollbackAsync();
                throw new Exception("An error occurred while adding the person.", ex);
            } finally {
            }

            throw new InvalidOperationException("An unexpected error occurred. The person could not be added.");
        }

        public async Task<(PersonDom, PersonDom, bool shouldSendEmail)> UpdateAsync(int personId, PersonDom updatedPerson) {
            var currentPerson = await GetPersonById(personId);

            if (currentPerson == null) {
                throw new PersonException("Person not found");
            }

            var originalPerson = _mapper.Map<PersonDom>(currentPerson);

            var isAddressUpdated = await UpdateAddressIfNeeded(currentPerson, updatedPerson.Address);
            var isSchoolUpdated = await UpdateSchoolIfNeeded(currentPerson, updatedPerson.School);

            bool shouldSendEmail = currentPerson.Gender != updatedPerson.Gender || DateOnly.FromDateTime(currentPerson.BirthDate) != updatedPerson.BirthDate;

            if (isAddressUpdated || isSchoolUpdated || !_mapper.Map<PersonDom>(currentPerson).Equals(updatedPerson)) {
                currentPerson.LastName = updatedPerson.LastName;
                currentPerson.FirstName = updatedPerson.FirstName;
                currentPerson.IBANNumber = updatedPerson.IBANNumber;
                currentPerson.BirthDate = updatedPerson.BirthDate.ToDateTime(TimeOnly.Parse("00:00:00"));
                currentPerson.Gender = updatedPerson.Gender;

                _context.People.Update(_mapper.Map<PersonEF>(currentPerson));

                await _context.SaveChangesAsync();
            }
            return (originalPerson, _mapper.Map<PersonDom>(currentPerson), shouldSendEmail);
        }

        public async Task UpdatePersonEmailAsync(int personId, PersonDom updatedPerson) {
            var currentPerson = await GetPersonById(personId);

            if (currentPerson == null) {
                throw new PersonException("Person not found");
            }

            try {
                currentPerson.Email = updatedPerson.Email;
                currentPerson.UserId = updatedPerson.UserId;

                _context.People.Update(_mapper.Map<PersonEF>(currentPerson));

                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                throw new Exception($"UpdatePersonEmailAsync: {ex.Message}");
            }
        }

        public Task<PersonDom> UpsertAsync(int? addressId, PersonDom updatedItem) {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(int personId) {
            var person = await GetPersonById(personId);

            if (person == null) {
                throw new PersonNotFoundException("This person doesn't exist anymore");
            }

            try {
                _context.Remove(person);
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                throw new Exception($"DeleteAsync: {ex.Message}");
            }
        }

        public async Task<PersonDom?> GetByGuidAsync(string userId) {
            try {
                if (Guid.TryParse(userId, out var userGuid)) {
                    var person = await _context.People
                        .FirstOrDefaultAsync(p => p.UserId == userGuid); 

                    return person == null ? null : _mapper.Map<PersonDom>(person);
                } else {
                    throw new ArgumentException("Invalid user id format.");
                }
            } catch (Exception ex) {
                throw new Exception($"GetByGuidAsync: {ex.Message}");
            }
        }


        private async Task<ExtendedPersonEF?> GetPersonById(int personId) {
            try {
                return _mapper.Map<ExtendedPersonEF?>(await GetByIdAsync(personId));
            } catch (Exception ex) {
                throw new Exception($"GetPersonById: {ex.Message}");
            }
        }

        private async Task<bool> UpdateAddressIfNeeded(ExtendedPersonEF person, AddressDom updatedAddress) {
            var currentAddress = _mapper.Map<AddressDom>(person.Address);

            if (!currentAddress.Equals(updatedAddress)) {
                var address = await _addressRepo.UpsertAsync(null, updatedAddress);
                person.AddressId = address.Id;
                person.Address = _mapper.Map<AddressEF>(address);
                return true;
            }

            return false;
        }

        private async Task<bool> UpdateSchoolIfNeeded(ExtendedPersonEF person, SchoolDom? updatedPersonSchool) {
            var currentSchool = _mapper.Map<SchoolDom>(person.School);

            if (currentSchool != null && updatedPersonSchool == null) {
                person.SchoolId = null;
                person.School = null;
                return true;
            }

            if ((currentSchool == null && updatedPersonSchool != null) ||
                (updatedPersonSchool != null && currentSchool != null && !currentSchool.Equals(updatedPersonSchool))) {
                var school = await _schoolRepo.UpsertAsync(null, updatedPersonSchool);
                person.SchoolId = school.Id;
                person.School = _mapper.Map<SchoolEF>(school);

                return true;
            }

            return false;
        }
    }
}