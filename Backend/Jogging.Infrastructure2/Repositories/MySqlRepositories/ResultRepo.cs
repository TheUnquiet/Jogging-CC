using AutoMapper;
using Jogging.Domain.Exceptions;
using Jogging.Domain.Helpers;
using Jogging.Domain.Interfaces.RepositoryInterfaces;
using Jogging.Domain.Models;
using Jogging.Infrastructure.Models.DatabaseModels.Result;
using Microsoft.EntityFrameworkCore;

namespace Jogging.Infrastructure2.Repositories.MySqlRepositories;

public class ResultRepo : IResultRepo {
    private readonly DbContext _context;
    private readonly CustomMemoryCache _memoryCache;
    private readonly IMapper _mapper;

    public ResultRepo(DbContext context, CustomMemoryCache memoryCache, IMapper mapper) {
        _context = context;
        _memoryCache = memoryCache;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ResultFunctionDom>> GetAllResults() {
        var allResults = await _context.Set<ResultEF>()
            .ToListAsync();

        if (!allResults.Any()) {
            throw new ResultException("No results found");
        }

        return _mapper.Map<IEnumerable<ResultFunctionDom>>(allResults);
    }

    public async Task<IQueryable<ResultDom>> GetPersonResultByIdAsync(int personId) {
        var results = await _context.Set<ResultEF>()
            .Where(r => r.PersonId == personId && r.RunTime != null && r.RunNumber != null)
            .ToListAsync();

        if (!results.Any()) {
            throw new ResultException("No results found");
        }

        return _mapper.Map<List<ResultDom>>(results).AsQueryable();
    }

    public async Task<IQueryable<ResultDom>> GetCompetitionResultByIdAsync(int competitionId) {
        var results = await _context.Set<ResultEF>()
            .Where(r => r.CompetitionId == competitionId && r.RunNumber != null)
            .ToListAsync();

        if (!results.Any()) {
            throw new ResultException("No results found");
        }

        return _mapper.Map<List<ResultDom>>(results).AsQueryable();
    }

    public async Task<IQueryable<ResultDom>> GetCompetitionResultByIdWithRunNumberAsync(int competitionId) {
        var results = await _context.Set<ResultEF>()
            .Where(r => r.CompetitionId == competitionId && r.RunNumber != null && r.RunTime != null)
            .OrderBy(r => r.RunTime)
            .ToListAsync();

        if (!results.Any()) {
            throw new ResultException("No valid results found");
        }

        return _mapper.Map<List<ResultDom>>(results).AsQueryable();
    }

    public async Task UpsertBulkAsync(List<ResultDom> registrations) {
        var entities = _mapper.Map<List<ResultEF>>(registrations);
        foreach (var entity in entities) {
            var existingEntity = await _context.Set<ResultEF>()
                .FirstOrDefaultAsync(e => e.Id == entity.Id);

            if (existingEntity != null) {
                _context.Entry(existingEntity).CurrentValues.SetValues(entity);
            } else {
                _context.Set<ResultEF>().Add(entity);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task UpdateRunTimeAsync(int registrationId, ResultDom updatedResult) {
        var registration = await _context.Set<ResultEF>()
            .FirstOrDefaultAsync(r => r.Id == registrationId);

        if (registration == null) {
            throw new ResultException("Result not found");
        }

        registration.RunTime = updatedResult.RunTime;
        await _context.SaveChangesAsync();

        _memoryCache.Remove(CacheKeyGenerator.GetCompetitionResultsKey(registration.CompetitionId));
        _memoryCache.Remove(CacheKeyGenerator.GetAllResultsKey());
    }
}