using Jogging.Domain.Models;

namespace Jogging.Domain.Interfaces.RepositoryInterfaces {
    public interface IClubRepo : IGenericRepo<ClubDom> {
        Task<ClubDom?> GetByNameAsync(string name);
        Task<List<ClubDom>> GetAllWithMembersAsync();
        Task<ClubDom?> GetClubByIdWithMembersAsync(int clubId);
    }
}
