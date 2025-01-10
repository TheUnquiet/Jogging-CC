using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Jogging.Domain.Enums;
using Jogging.Domain.Exceptions;
using Jogging.Domain.Interfaces.RepositoryInterfaces;
using Jogging.Domain.Models;
using Jogging.Infrastructure2.Data;
using Jogging.Infrastructure2.Models;
using Microsoft.EntityFrameworkCore;

namespace Jogging.Infrastructure2.Repositories.MySqlRepositories {
    public class CompetitionPerCategoryRepo : ICompetitionPerCategoryRepo {
        private readonly JoggingCcContext _dbContext;
        private readonly IAgeCategoryRepo _ageCategoryRepo;
        private readonly IMapper _mapper;

        public CompetitionPerCategoryRepo(JoggingCcContext dbContext, IAgeCategoryRepo ageCategoryRepo, IMapper mapper) {
            _dbContext = dbContext;
            _ageCategoryRepo = ageCategoryRepo;
            _mapper = mapper;
        }

        public async Task<List<CompetitionPerCategoryDom>> UpdateAsync(Dictionary<string, float> distances, int competitionId) {
            var competitionPerCategories = await _dbContext.CompetitionPerCategories
                .Where(cpc => cpc.CompetitionId == competitionId)
                .ToListAsync();

            foreach (var distance in distances) {
                var categoryToUpdate = competitionPerCategories.FirstOrDefault(cpc => cpc.DistanceName == distance.Key);
                if (categoryToUpdate != null) {
                    categoryToUpdate.DistanceInKm = distance.Value;
                }
            }

            await _dbContext.SaveChangesAsync();

            return _mapper.Map<List<CompetitionPerCategoryDom>>(competitionPerCategories);
        }

        public async Task DeleteAsync(int competitionPerCategoryId) {
            var competitionPerCategory = await _dbContext.CompetitionPerCategories
                .FirstOrDefaultAsync(cpc => cpc.Id == competitionPerCategoryId);

            if (competitionPerCategory == null) {
                throw new CompetitionException("Competition per category not found");
            }

            _dbContext.CompetitionPerCategories.Remove(competitionPerCategory);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<CompetitionPerCategoryDom>> AddAsync(Dictionary<string, float> distances, int competitionId) {
            var ageCategories = await _ageCategoryRepo.GetAllAsync();
            List<CompetitionPerCategoryEF> competitionPerCategories = new List<CompetitionPerCategoryEF>();

            foreach (var ageCategory in ageCategories) {
                foreach (var distance in distances.OrderBy(d => d.Value)) {
                    foreach (var gender in (Genders[])Enum.GetValues(typeof(Genders))) {
                        competitionPerCategories.Add(new CompetitionPerCategoryEF {
                            DistanceName = distance.Key,
                            DistanceInKm = distance.Value,
                            AgeCategoryId = ageCategory.Id,
                            CompetitionId = competitionId,
                            Gender = gender.ToString(),
                        });
                    }
                }
            }

            await _dbContext.CompetitionPerCategories.AddRangeAsync(competitionPerCategories);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<List<CompetitionPerCategoryDom>>(competitionPerCategories);
        }

        public async Task UpdateGunTimeAsync(int competitionId, DateTime gunTime) {
            var competitionPerCategories = await _dbContext.CompetitionPerCategories
                .Where(cpc => cpc.CompetitionId == competitionId)
                .ToListAsync();

            if (!competitionPerCategories.Any()) {
                throw new CompetitionException("Competition not found");
            }

            competitionPerCategories.ForEach(cpc => cpc.GunTime = gunTime);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<CompetitionPerCategoryDom> GetCompetitionPerCategoryByParameters(int ageCategoryId, string distanceName, char personGender, int competitionId) {
            var competitionPerCategory = await _dbContext.CompetitionPerCategories
                .FirstOrDefaultAsync(cpc =>
                    cpc.AgeCategoryId == ageCategoryId &&
                    cpc.DistanceName == distanceName &&
                    cpc.CompetitionId == competitionId &&
                    cpc.Gender == personGender.ToString().ToUpper());

            if (competitionPerCategory == null) {
                throw new CompetitionException("This competition per category doesn't exist");
            }

            return _mapper.Map<CompetitionPerCategoryDom>(competitionPerCategory);
        }

        public async Task<CompetitionPerCategoryDom> AddAsync(CompetitionPerCategoryDom person) {
            if (person == null) {
                throw new ArgumentNullException(nameof(person), "Person cannot be null");
            }

            var competitionPerCategory = new CompetitionPerCategoryEF {
                DistanceName = person.DistanceName,
                DistanceInKm = person.DistanceInKm,
                AgeCategoryId = person.AgeCategoryId,
                CompetitionId = person.CompetitionId,
                Gender = person.Gender.ToString(),
            };

            await _dbContext.CompetitionPerCategories.AddAsync(competitionPerCategory);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<CompetitionPerCategoryDom>(competitionPerCategory);
        }

        public async Task<List<CompetitionPerCategoryDom>> GetAllAsync() {
            var competitionPerCategories = await _dbContext.CompetitionPerCategories.ToListAsync();
            return _mapper.Map<List<CompetitionPerCategoryDom>>(competitionPerCategories);
        }

        public async Task<CompetitionPerCategoryDom> GetByIdAsync(int id) {
            var competitionPerCategory = await _dbContext.CompetitionPerCategories
                .FirstOrDefaultAsync(cpc => cpc.Id == id);

            if (competitionPerCategory == null) {
                throw new CompetitionException("Competition per category not found");
            }

            return _mapper.Map<CompetitionPerCategoryDom>(competitionPerCategory);
        }

        public async Task<CompetitionPerCategoryDom> UpdateAsync(int id, CompetitionPerCategoryDom updatedItem) {
            if (updatedItem == null) {
                throw new ArgumentNullException(nameof(updatedItem), "Updated item cannot be null");
            }

            var competitionPerCategory = await _dbContext.CompetitionPerCategories
                .FirstOrDefaultAsync(cpc => cpc.Id == id);

            if (competitionPerCategory == null) {
                throw new CompetitionException("Competition per category not found");
            }

            competitionPerCategory.DistanceName = updatedItem.DistanceName;
            competitionPerCategory.DistanceInKm = updatedItem.DistanceInKm;
            competitionPerCategory.AgeCategoryId = updatedItem.AgeCategoryId;
            competitionPerCategory.Gender = updatedItem.Gender.ToString();

            _dbContext.CompetitionPerCategories.Update(competitionPerCategory);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<CompetitionPerCategoryDom>(competitionPerCategory);
        }

        public async Task<CompetitionPerCategoryDom> UpsertAsync(int? id, CompetitionPerCategoryDom updatedItem) {
            if (updatedItem == null) {
                throw new ArgumentNullException(nameof(updatedItem), "Updated item cannot be null");
            }

            if (id == null) {
                return await AddAsync(updatedItem);
            }

            var competitionPerCategory = await _dbContext.CompetitionPerCategories
                .FirstOrDefaultAsync(cpc => cpc.Id == id);

            if (competitionPerCategory == null) {
                return await AddAsync(updatedItem);
            }

            competitionPerCategory.DistanceName = updatedItem.DistanceName;
            competitionPerCategory.DistanceInKm = updatedItem.DistanceInKm;
            competitionPerCategory.AgeCategoryId = updatedItem.AgeCategoryId;
            competitionPerCategory.Gender = updatedItem.Gender.ToString(); 

            _dbContext.CompetitionPerCategories.Update(competitionPerCategory);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<CompetitionPerCategoryDom>(competitionPerCategory);
        }
    }
}
