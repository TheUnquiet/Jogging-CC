using AutoMapper;
using Jogging.Domain.DomainManagers;
using Jogging.Domain.Exceptions;
using Jogging.Domain.Helpers;
using Jogging.Domain.Interfaces.RepositoryInterfaces;
using Jogging.Domain.Models;
using Jogging.Infrastructure.Models.DatabaseModels.Address;
using Jogging.Infrastructure.Models.DatabaseModels.Competition;
using Jogging.Infrastructure.Models.DatabaseModels.CompetitionPerCategory;
using Microsoft.Extensions.Caching.Memory;
using Postgrest;
using Postgrest.Interfaces;

namespace Jogging.Infrastructure.Repositories.SupabaseRepos
{
    public class CompetitionRepo : ICompetitionRepo
    {
        private readonly Supabase.Client _client;
        private readonly IGenericRepo<AddressDom> _addressRepo;
        private readonly ICompetitionPerCategoryRepo _competitionPerCategoryRepo;
        private readonly IMapper _mapper;
        private readonly CustomMemoryCache _memoryCache;

        public CompetitionRepo(Supabase.Client client, IGenericRepo<AddressDom> addressRepo, ICompetitionPerCategoryRepo competitionPerCategoryRepo,
            IMapper mapper, CustomMemoryCache memoryCache)
        {
            _client = client;
            _addressRepo = addressRepo;
            _competitionPerCategoryRepo = competitionPerCategoryRepo;
            _mapper = mapper;
            _memoryCache = memoryCache;
        }

        public async Task<List<CompetitionDom>> GetAllAsync()
        {
            var competitions = await _client
                .From<SimpleCompetition>()
                .Order(competition => competition.Date, Constants.Ordering.Ascending)
                .Get();

            if (competitions.Models.Count <= 0)
            {
                throw new CompetitionException("No competitions found");
            }

            return _mapper.Map<List<CompetitionDom>>(competitions.Models);
        }

        public async Task<List<CompetitionDom>> GetAllWithSearchValuesAsync(string? competitionName, DateOnly? startDate, DateOnly? endDate)
        {
            var filters = new List<IPostgrestQueryFilter>();

            if (!string.IsNullOrEmpty(competitionName))
            {
                filters.Add(new QueryFilter("Name", Constants.Operator.ILike, $"%{competitionName}%"));
            }

            if (startDate.HasValue)
            {
                var startDateTime = startDate.Value.ToDateTime(TimeOnly.MinValue);
                filters.Add(new QueryFilter("Date", Constants.Operator.GreaterThanOrEqual, startDateTime));
            }

            if (endDate.HasValue)
            {
                var endDateTime = endDate.Value.ToDateTime(TimeOnly.MinValue);
                filters.Add(new QueryFilter("Date", Constants.Operator.LessThanOrEqual, endDateTime));
            }
            
            var competitions = await _client
                .From<SimpleCompetition>()
                .And(filters)
                .Order(competition => competition.Date, Constants.Ordering.Ascending)
                .Get();

            if (competitions.Models.Count <= 0)
            {
                throw new CompetitionException("No competitions found");
            }

            return _mapper.Map<List<CompetitionDom>>(competitions.Models);
        }

        public async Task<List<CompetitionDom>> GetAllActiveAsync()
        {
            var competitions = await _client
                .From<SimpleCompetition>()
                .Where(c => c.Active == true)
                .Order(competition => competition.Date, Constants.Ordering.Ascending)
                .Get();

            if (competitions.Models.Count <= 0)
            {
                throw new CompetitionException("No competitions found");
            }

            return _mapper.Map<List<CompetitionDom>>(competitions.Models);
        }

        public async Task<List<CompetitionDom>> GetAllActiveWithSearchValuesAsync(string? competitionName, DateOnly? startDate, DateOnly? endDate)
        {
            var competitions = await _client
                .From<SimpleCompetition>()
                .Where(c => c.Active == true)
                .Filter(c => c.Name, Constants.Operator.Like, $"%{competitionName}%")
                .Order(competition => competition.Date, Constants.Ordering.Ascending)
                .Get();

            if (competitions.Models.Count <= 0)
            {
                throw new CompetitionException("No competitions found");
            }

            return _mapper.Map<List<CompetitionDom>>(competitions.Models);
        }

        public async Task<List<CompetitionDom>> GetAllActiveRankingAsync()
        {
            var competitions = await _client
                .From<SimpleCompetition>()
                .Where(c => c.RankingActive == true)
                .Order(competition => competition.Date, Constants.Ordering.Ascending)
                .Get();

            if (competitions.Models.Count <= 0)
            {
                throw new CompetitionException("No competitions found");
            }

            return _mapper.Map<List<CompetitionDom>>(competitions.Models);
        }

        public async Task<CompetitionDom> GetSimpleCompetitionByIdAsync(int competitionId)
        {
            var competition = await _client
                .From<SimpleCompetition>()
                .Filter("Id", Constants.Operator.Equals, competitionId)
                .Limit(1)
                .Single();

            if (competition == null)
            {
                throw new CompetitionException("Competition not found");
            }

            return _mapper.Map<CompetitionDom>(competition);
        }

        public async Task<CompetitionDom> GetByIdAsync(int competitionId)
        {
            var competition = await GetExtendedCompetitionById(competitionId);

            return _mapper.Map<CompetitionDom>(competition);
        }

        public async Task<CompetitionDom> AddAsync(CompetitionDom competitionDom)
        {
            var address = await _addressRepo.UpsertAsync(null, _mapper.Map<AddressDom>(competitionDom.Address));

            competitionDom.AddressId = address.Id;
            competitionDom.Address = address;

            var response = await _client
                .From<SimpleCompetition>()
                .Insert(_mapper.Map<SimpleCompetition>(competitionDom));

            if (response.Model?.Id == null)
            {
                throw new CompetitionException("Something went wrong adding competition");
            }

            if (competitionDom.Distances != null)
            {
                var competitionPerCategories = await _competitionPerCategoryRepo.AddAsync(competitionDom.Distances, response.Model.Id);
                competitionDom.CompetitionPerCategories = competitionPerCategories;
            }

            if (competitionDom.RankingActive)
            {
                _memoryCache.Remove(CacheKeyGenerator.GetAllResultsKey());
            }

            competitionDom.Id = response.Model.Id;
            return competitionDom;
        }

        public async Task<CompetitionDom> UpdateAsync(int competitionId, CompetitionDom updatedCompetition)
        {
            var currentCompetition = await GetExtendedCompetitionById(competitionId);

            var isAddressUpdated = await UpdateAddressIfNeeded(currentCompetition, updatedCompetition.Address);
            var isDistancesUpdated = await UpdateDistancesIfNeeded(currentCompetition, updatedCompetition);

            if (isAddressUpdated || isDistancesUpdated || !_mapper.Map<CompetitionDom>(currentCompetition).Equals(updatedCompetition))
            {
                currentCompetition.Name = updatedCompetition.Name;
                currentCompetition.Information = updatedCompetition.Information;
                currentCompetition.Date = updatedCompetition.Date;
                currentCompetition.Active = updatedCompetition.Active;
                if (currentCompetition.RankingActive != updatedCompetition.RankingActive)
                {
                    _memoryCache.Remove(CacheKeyGenerator.GetAllResultsKey());
                }

                currentCompetition.RankingActive = updatedCompetition.RankingActive;

                var competition = await currentCompetition.Update<ExtendedCompetition>();

                if (competition.Model == null)
                {
                    throw new CompetitionException("Something went wrong while updating your competition");
                }

                return _mapper.Map<CompetitionDom>(competition.Model);
            }

            return _mapper.Map<CompetitionDom>(currentCompetition);
        }

        public async Task DeleteAsync(int competitionId)
        {
            await _client
                .From<SimpleCompetition>()
                .Where(c => c.Id == competitionId)
                .Delete();
        }

        private async Task<ExtendedCompetition> GetExtendedCompetitionById(int competitionId)
        {
            var competition = await _client
                .From<ExtendedCompetition>()
                .Filter("Id", Constants.Operator.Equals, competitionId)
                .Limit(1)
                .Single();

            if (competition == null)
            {
                throw new CompetitionException("Competition not found");
            }

            return competition;
        }

        private async Task<bool> UpdateAddressIfNeeded(ExtendedCompetition competition, AddressDom updatedAddress)
        {
            var currentAddress = _mapper.Map<AddressDom>(competition.Address);

            if (!currentAddress.Equals(updatedAddress))
            {
                var address = await _addressRepo.UpsertAsync(null, updatedAddress);
                competition.AddressId = address.Id;
                competition.Address = _mapper.Map<SimpleAddress>(address);
                return true;
            }

            return false;
        }

        private async Task<bool> UpdateDistancesIfNeeded(ExtendedCompetition competition, CompetitionDom updatedCompetitionDom)
        {
            var existingDistances = CompetitionManager.GetDistances(_mapper.Map<CompetitionDom>(competition));
            if (!DictionaryHelper.AreDictionariesEqual(updatedCompetitionDom.Distances, existingDistances))
            {
                var competitionPerCategories = await _competitionPerCategoryRepo.UpdateAsync(updatedCompetitionDom.Distances, competition.Id);
                competition.CompetitionPerCategories = _mapper.Map<List<ExtendedCompetitionPerCategory>>(competitionPerCategories);
                return true;
            }

            return false;
        }

        public Task<CompetitionDom> UpsertAsync(int? addressId, CompetitionDom updatedItem)
        {
            throw new NotImplementedException();
        }
    }
}