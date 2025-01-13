using AutoMapper;
using Jogging.Domain.Interfaces.RepositoryInterfaces;
using Jogging.Domain.Models;
using Jogging.Infrastructure2.Data;
using Jogging.Infrastructure2.Models.Club;
using Microsoft.EntityFrameworkCore;

namespace Jogging.Infrastructure2.Repositories.MySqlRepositories {
    public class ClubRepo : IClubRepo {
        private readonly JoggingCcContext _context;
        private readonly IMapper _mapper;

        public ClubRepo(JoggingCcContext context, IMapper mapper) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ClubDom> AddAsync(ClubDom newClub) {
            if (newClub == null) throw new ArgumentNullException(nameof(newClub));

            var club = _mapper.Map<ClubEF>(newClub) ?? throw new Exception("Club is null");

            if (await _context.Clubs.AnyAsync(c => c.Name == newClub.Name)) {
                throw new Exception("Club already exists");
            }

            await _context.Clubs.AddAsync(club);
            await _context.SaveChangesAsync();
            return newClub;
        }

        public async Task DeleteAsync(int id) {
            var club = await _context.Clubs.FindAsync(id) ?? throw new Exception("Club does not exist");
            _context.Clubs.Remove(club);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ClubDom>> GetAllAsync() {
            return await _context.Clubs
                .AsNoTracking()
                .Select(c => _mapper.Map<ClubDom>(c))
                .ToListAsync();
        }

        public async Task<ClubDom> GetByIdAsync(int id) {
            return await _context.Clubs
                .Where(c => c.Id == id)
                .AsNoTracking()
                .Select(c => _mapper.Map<ClubDom>(c))
                .FirstOrDefaultAsync() ?? throw new Exception("Club does not exist");
        }

        public async Task<ClubDom?> GetByNameAsync(string name) {
            return await _context.Clubs
                .Where(c => c.Name == name)
                .AsNoTracking()
                .Select(c => _mapper.Map<ClubDom?>(c))
                .FirstOrDefaultAsync();
        }

        public async Task<ClubDom?> GetClubByIdWithMembersAsync(int clubId) {
            return await _context.Clubs
                .Where(c => c.Id == clubId)
                .Include(c => c.Members)
                .AsNoTracking()
                .Select(c => _mapper.Map<ClubDom?>(c))
                .FirstOrDefaultAsync();
        }

        public async Task<ClubDom> UpdateAsync(int id, ClubDom updatedItem) {
            if (updatedItem == null) throw new ArgumentNullException(nameof(updatedItem));

            var existingClub = await _context.Clubs.FindAsync(id);
            if (existingClub == null) throw new Exception("Club does not exist");

            _mapper.Map(updatedItem, existingClub);
            await _context.SaveChangesAsync();

            return _mapper.Map<ClubDom>(existingClub);
        }
    }
}