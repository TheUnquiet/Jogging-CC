using Jogging.Domain.Models;
using Jogging.Domain.Interfaces.RepositoryInterfaces;
using Jogging.Domain.Helpers;

namespace Jogging.Domain.DomainManagers {
    public class ClubManager {
        private readonly IClubRepo _clubRepo;

        public ClubManager(IClubRepo clubRepo) {
            _clubRepo = clubRepo;
        }

        public async Task<PagedList<ClubDom>> GetAllAsync(QueryStringParameters parameters) {
            var clubs = await _clubRepo.GetAllAsync();
            return PagedList<ClubDom>.ToPagedList(clubs.AsQueryable(), parameters.PageNumber, parameters.PageSize);
        }

        public async Task<ClubDom?> GetByIdAsync(int clubId) {
            return await _clubRepo.GetByIdAsync(clubId);
        }

        public async Task<ClubDom?> GetByIdWithMembersAsync(int clubId) {
            return await _clubRepo.GetClubByIdWithMembersAsync(clubId);
        }

        public async Task<ClubDom> CreateAsync(ClubDom club) {
            return await _clubRepo.AddAsync(club);
        }

        public async Task<ClubDom> UpdateAsync(int clubId, ClubDom updatedClub) {
            return await _clubRepo.UpdateAsync(clubId, updatedClub);
        }

        public async Task DeleteAsync(int clubId) {
            await _clubRepo.DeleteAsync(clubId);
        }
    }
}
