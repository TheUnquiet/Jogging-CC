using AutoMapper;
using Jogging.Domain.Exceptions;
using Jogging.Domain.Interfaces.RepositoryInterfaces;
using Jogging.Domain.Models;
using Jogging.Infrastructure2.Data;
using Microsoft.EntityFrameworkCore;

namespace Jogging.Infrastructure2.Repositories.MySqlRepositories {
    public class ProfileRepo : IProfileRepo {
        private readonly JoggingCcContext _context;
        private readonly IMapper _mapper;

        public ProfileRepo(JoggingCcContext context, IMapper mapper) {
            _context = context;
            _mapper = mapper;
        }

        public async Task UpdateAsync(string userId, ProfileDom updatedItem) {
            if (!Guid.TryParse(userId, out var userGuid)) {
                throw new ArgumentException("Invalid user id format.");
            }

            var profile = await _context.Profiles
                .FirstOrDefaultAsync(p => p.Id == userGuid);

            if (profile == null) {
                throw new ProfileException("Profile not found.");
            }

            try {
                profile.Role = updatedItem.Role;

                _context.Profiles.Update(profile);

                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                throw new Exception($"Error during profile update: {ex.Message}");
            }
        }
    }
}