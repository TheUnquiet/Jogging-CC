using Jogging.Domain.Models;
using Jogging.Domain.Interfaces.RepositoryInterfaces;

namespace Jogging.Domain.DomainManagers {
    public class ClubManager {
        private readonly IClubRepo _clubRepo;

        public ClubManager(IClubRepo clubRepo) {
            _clubRepo = clubRepo ?? throw new ArgumentNullException(nameof(clubRepo));
        }

        public async Task<List<ClubDom>> GetAllAsync() {
            return await _clubRepo.GetAllAsync();
        }

        public async Task<ClubDom?> GetByIdAsync(int clubId) {
            return await _clubRepo.GetByIdAsync(clubId);
        }

        public async Task<ClubDom?> GetByIdWithMembersAsync(int clubId) {
            return await _clubRepo.GetClubByIdWithMembersAsync(clubId);
        }

        public async Task<ClubDom> CreateAsync(ClubDom club) {
            if (club == null) throw new ArgumentNullException(nameof(club));
            return await _clubRepo.AddAsync(club);
        }

        public async Task<ClubDom> UpdateAsync(int clubId, ClubDom updatedClub) {
            if (updatedClub == null) throw new ArgumentNullException(nameof(updatedClub));
            return await _clubRepo.UpdateAsync(clubId, updatedClub);
        }

        public async Task DeleteAsync(int clubId) {
            await _clubRepo.DeleteAsync(clubId);
        }
    }
}
