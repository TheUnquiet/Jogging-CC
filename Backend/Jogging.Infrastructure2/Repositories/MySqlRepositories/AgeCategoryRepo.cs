using AutoMapper;
using Jogging.Domain.Exceptions;
using Jogging.Domain.Interfaces.RepositoryInterfaces;
using Jogging.Domain.Models;
using Jogging.Infrastructure2.Data;
using Jogging.Infrastructure2.Models;
using Microsoft.EntityFrameworkCore;

namespace Jogging.Infrastructure2.Repositories.MySqlRepositories {

    public class AgeCategoryRepository : IAgeCategoryRepo {
        private readonly JoggingCcContext _context;
        private readonly IMapper _mapper;

        public AgeCategoryRepository(JoggingCcContext context, IMapper mapper) {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<AgeCategoryDom>> GetAllAsync() {
            var ageCategories = await _context.AgeCategories
                .ToListAsync();

            if (ageCategories.Count == 0) {
                throw new AgeCategoryException("No age categories found");
            }

            return _mapper.Map<List<AgeCategoryDom>>(ageCategories);
        }

        public async Task<AgeCategoryDom> GetByIdAsync(int ageCategoryId) {
            var ageCategory = await _context.AgeCategories
                .FirstOrDefaultAsync(ac => ac.Id == ageCategoryId);

            return ageCategory == null
                ? throw new AgeCategoryException("Age category not found")
                : _mapper.Map<AgeCategoryDom>(ageCategory);
        }

        public async Task<AgeCategoryDom> GetAgeCategoryByAge(PersonDom person) {
            var ageCategory = await _context.AgeCategories
                .FirstOrDefaultAsync(ageCat =>
                    ageCat.MaximumAge >= person.BirthYearAge &&
                    ageCat.MinimumAge <= person.BirthYearAge);

            return ageCategory == null
                ? throw new AgeCategoryException("Age category not found")
                : _mapper.Map<AgeCategoryDom>(ageCategory);
        }

        public async Task<AgeCategoryDom> AddAsync(AgeCategoryDom ageCategoryDom) {
            var ageCategory = _mapper.Map<AgeCategoryEF>(ageCategoryDom);

            _context.AgeCategories.Add(ageCategory);
            await _context.SaveChangesAsync();

            return _mapper.Map<AgeCategoryDom>(ageCategory);
        }

        public async Task<AgeCategoryDom> UpdateAsync(int id, AgeCategoryDom updatedItem) {
            var existingCategory = await _context.AgeCategories
                .FirstOrDefaultAsync(ac => ac.Id == id);

            if (existingCategory == null) {
                throw new AgeCategoryException("Age category not found");
            }

            var mappedCategory = _mapper.Map<AgeCategoryEF>(updatedItem);

            existingCategory.Name = mappedCategory.Name;
            existingCategory.MinimumAge = mappedCategory.MinimumAge;
            existingCategory.MaximumAge = mappedCategory.MaximumAge;

            await _context.SaveChangesAsync();

            return _mapper.Map<AgeCategoryDom>(existingCategory);
        }

        public async Task<AgeCategoryDom> UpsertAsync(int? id, AgeCategoryDom updatedItem) {
            if (id.HasValue) {
                var existingCategory = await _context.AgeCategories
                    .FirstOrDefaultAsync(ac => ac.Id == id.Value);

                if (existingCategory != null) {
                    var mappedCategory = _mapper.Map<AgeCategoryEF>(updatedItem);

                    existingCategory.Name = mappedCategory.Name;
                    existingCategory.MinimumAge = mappedCategory.MinimumAge;
                    existingCategory.MaximumAge = mappedCategory.MaximumAge;

                    await _context.SaveChangesAsync();

                    return _mapper.Map<AgeCategoryDom>(existingCategory);
                }
            }

            return await AddAsync(updatedItem);
        }

        public async Task DeleteAsync(int id) {
            var ageCategory = await _context.AgeCategories
                .FirstOrDefaultAsync(ac => ac.Id == id);

            if (ageCategory == null) {
                throw new AgeCategoryException("Age category not found");
            }

            _context.AgeCategories.Remove(ageCategory);
            await _context.SaveChangesAsync();
        }
    }
}