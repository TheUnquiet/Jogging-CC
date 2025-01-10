using AutoMapper;
using Jogging.Domain.Interfaces.RepositoryInterfaces;
using Jogging.Domain.Models;
using Jogging.Infrastructure.Models.DatabaseModels.Club;
using Jogging.Infrastructure.Models.DatabaseModels.Person;
using Microsoft.Extensions.Logging;
using Postgrest;
using Client = Supabase.Client;

namespace Jogging.Infrastructure.Repositories.SupabaseRepos {
    public class ClubRepo : IClubRepo {
        private readonly Client _client;
        private readonly IMapper _mapper;
        private readonly ILogger<ClubRepo> _logger;

        public ClubRepo(Client client, IMapper mapper, ILogger<ClubRepo> logger) {
            _client = client;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<ClubDom>> GetAllAsync() {
            var clubs = await _client.From<SimpleClub>().Get();

            if (clubs.Models.Count <= 0) {
                _logger.LogWarning("No clubs found.");
                return new List<ClubDom>();
            }

            return _mapper.Map<List<ClubDom>>(clubs.Models);
        }

        public async Task<ClubDom?> GetByIdAsync(int clubId) {
            var club = await _client.From<ExtendedClub>()
                .Where(c => c.Id == clubId)
                .Single();

            if (club == null) {
                _logger.LogWarning($"Club with id {clubId} not found.");
                return null;
            }

            return _mapper.Map<ClubDom>(club);
        }

        public async Task<ClubDom?> GetByNameAsync(string name) {
            var club = await _client.From<SimpleClub>()
                .Where(c => c.Name == name)
                .Single();

            return club != null ? _mapper.Map<ClubDom>(club) : null;
        }

        public async Task<List<ClubDom>> GetAllWithMembersAsync() {
            var clubs = await _client.From<ExtendedClub>().Get();

            if (clubs.Models.Count <= 0) {
                return new List<ClubDom>();
            }

            var result = new List<ClubDom>();
            foreach (var club in clubs.Models) {
                var members = await _client.From<ExtendedPerson>()
                    .Where(p => p.LoopClubId == club.Id)
                    .Get();

                var clubDom = _mapper.Map<ClubDom>(club);
                clubDom.Members = _mapper.Map<List<PersonDom>>(members.Models);
                result.Add(clubDom);
            }

            return result;
        }

        public async Task<ClubDom?> GetClubByIdWithMembersAsync(int clubId) {
            var club = await _client.From<ExtendedClub>()
                .Where(c => c.Id == clubId)
                .Single();

            if (club == null) {
                return null;
            }

            var members = await _client.From<ExtendedPerson>()
                .Where(p => p.LoopClubId == club.Id)
                .Get();

            var clubDom = _mapper.Map<ClubDom>(club);
            clubDom.Members = _mapper.Map<List<PersonDom>>(members.Models);
            return clubDom;
        }

        public async Task<ClubDom> AddAsync(ClubDom newClub) {
            var club = _mapper.Map<SimpleClub>(newClub);

            var response = await _client.From<SimpleClub>()
                .Insert(club);

            if (response.Model == null) {
                throw new Exception("Failed to create new club.");
            }

            return _mapper.Map<ClubDom>(response.Model);
        }

        public async Task<ClubDom> UpdateAsync(int clubId, ClubDom updatedClub) {
            var currentClub = await GetByIdAsync(clubId);

            if (currentClub == null) {
                throw new Exception($"Club with id {clubId} not found.");
            }

            var club = _mapper.Map<ExtendedClub>(updatedClub);

            var response = await _client.From<ExtendedClub>()
                .Update(club);

            if (response.Model == null) {
                throw new Exception("Failed to update the club.");
            }

            return _mapper.Map<ClubDom>(response.Model);
        }

        public async Task DeleteAsync(int clubId) {
            var club = await _client.From<SimpleClub>()
                .Where(c => c.Id == clubId)
                .Single();

            if (club == null) {
                throw new Exception($"Club with id {clubId} not found.");
            }

            await club.Delete<SimpleClub>();
        }

        public async Task<ClubDom> UpsertAsync(int? id, ClubDom updatedItem) {
            if (id == null) {
                return await AddAsync(updatedItem);
            } else {
                return await UpdateAsync(id.Value, updatedItem);
            }
        }
    }
}
