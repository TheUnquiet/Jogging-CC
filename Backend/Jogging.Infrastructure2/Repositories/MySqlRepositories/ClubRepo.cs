using AutoMapper;
using Jogging.Domain.Interfaces.RepositoryInterfaces;
using Jogging.Domain.Models;
using Jogging.Infrastructure2.Data;
using Jogging.Infrastructure2.Models.Club;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jogging.Infrastructure2.Repositories.MySqlRepositories
{
    public class ClubRepo : IClubRepo
    {
        private JoggingCcContext _context;
        private IMapper _mapper; 

        public ClubRepo(JoggingCcContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ClubDom> AddAsync(ClubDom newClub)
        {
            var club = _mapper.Map<ClubEF>(newClub) ?? throw new Exception("Club is null");

            if (_context.Clubs.Contains(club))
            {
                throw new Exception("Club already exists");
            }

            await _context.Clubs.AddAsync(club);
            await _context.SaveChangesAsync();
            return newClub;
        }

        public async Task DeleteAsync(int id)
        {
            var club = await _context.Clubs
                .FindAsync(id) ?? throw new Exception("Club does not exist");

            _context.Clubs.Remove(club);
        }

        public async Task<List<ClubDom>> GetAllAsync()
        {
            return await _context.Clubs
                .Select((c) => _mapper.Map<ClubDom>(c))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<ClubDom>> GetAllWithMembersAsync()
        {
            return await _context.Clubs
                .Include((c) => c.Members)
                .Select((c) => _mapper.Map<ClubDom>(c))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ClubDom> GetByIdAsync(int id)
        {
            return await _context.Clubs
                .Where((c) => c.Id == id)
                .Select((c) => _mapper.Map<ClubDom>(c))
                .AsNoTracking()
                .FirstOrDefaultAsync()
                ?? throw new Exception("Club does not exist");
        }

        public async Task<ClubDom?> GetByNameAsync(string name)
        {
            return await _context.Clubs.Where((c) => c.Name == name)
                .Select((c) => _mapper.Map<ClubDom?>(c))
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<ClubDom?> GetClubByIdWithMembersAsync(int clubId)
        {
            return await _context.Clubs
                .Where((c) => c.Id == clubId)
                .Include((c) => c.Members)
                .Select((c) => _mapper.Map<ClubDom?>(c))
                .AsNoTracking()
                .FirstOrDefaultAsync()
                ?? throw new Exception("Club does not exist");
        }

        public async Task<ClubDom> UpdateAsync(int id, ClubDom updatedItem)
        {
            var club = await _context.Clubs
                .FindAsync(id) ?? throw new Exception("Club does not exist");

            _context.Clubs.Update(_mapper.Map<ClubEF>(updatedItem));
            await _context.SaveChangesAsync();
            return _mapper.Map<ClubDom>(club);
        }

        public Task<ClubDom> UpsertAsync(int? id, ClubDom updatedItem)
        {
            if (id == null) return AddAsync(updatedItem);
            return UpdateAsync(id.Value, updatedItem);
        }
    }
}
