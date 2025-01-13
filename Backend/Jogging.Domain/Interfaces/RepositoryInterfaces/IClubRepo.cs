using Jogging.Domain.Models;

namespace Jogging.Domain.Interfaces.RepositoryInterfaces {
    public interface IClubRepo {
        Task<List<ClubDom>> GetAllAsync();
        Task<ClubDom?> GetByIdAsync(int id);
        Task<ClubDom?> GetByNameAsync(string name);
        Task<ClubDom?> GetClubByIdWithMembersAsync(int clubId);
        Task<ClubDom> AddAsync(ClubDom newClub);
        Task<ClubDom> UpdateAsync(int id, ClubDom updatedItem);
        Task DeleteAsync(int id);
    }
}
