using Jogging.Infrastructure2.Models;
using Jogging.Infrastructure.Models.DatabaseModels.AgeCategory;
using Jogging.Infrastructure2.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataBaseSeeder {
    public class SeedManager {
        private readonly JoggingCcContext _context;

        public SeedManager(JoggingCcContext context) {
            _context = context;
        }

        public void Seed() {
            // Seed the Age Categories
            SeedAgeCategories();
        }

        private void DeleteAgeCategories() {
            var ageCategories = _context.AgeCategories.ToList();
            _context.AgeCategories.RemoveRange(ageCategories);
            _context.SaveChanges();
        }

        private void SeedAgeCategories() {
            if (!_context.AgeCategories.Any()) {
                List<SimpleAgeCategory> newAgeCategories = new()
                {
                    new SimpleAgeCategory() { Name = "min40", MinimumAge = 0, MaximumAge = 39 },
                    new SimpleAgeCategory() { Name = "plus40", MinimumAge = 40, MaximumAge = 49 },
                    new SimpleAgeCategory() { Name = "plus50", MinimumAge = 50, MaximumAge = 59 },
                    new SimpleAgeCategory() { Name = "plus60", MinimumAge = 60, MaximumAge = 150 }
                };

                var ageCategoryEFs = newAgeCategories.Select(ageCategory => new AgeCategoryEF {
                    Name = ageCategory.Name,
                    MinimumAge = ageCategory.MinimumAge,
                    MaximumAge = ageCategory.MaximumAge
                }).ToList();

                _context.AgeCategories.AddRange(ageCategoryEFs);
                _context.SaveChanges();
            }
        }

        public void Reset() {
            DeleteAgeCategories();
            Seed();
        }
    }
}