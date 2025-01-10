using AutoMapper;
using Jogging.Domain.Exceptions;
using Jogging.Domain.Interfaces.RepositoryInterfaces;
using Jogging.Domain.Models;
using Jogging.Infrastructure2.Data;
using Jogging.Infrastructure2.Models;
using Microsoft.EntityFrameworkCore;

namespace Jogging.Infrastructure2.Repositories.MySqlRepositories;

public class SchoolRepo : IGenericRepo<SchoolDom>
{
    private readonly JoggingCcContext _context;
    private readonly IMapper _mapper;

    public SchoolRepo(JoggingCcContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<SchoolDom>> GetAllAsync()
    {
        try
        {
            return _mapper.Map<List<SchoolDom>>(await _context.Schools.ToListAsync());
        }
        catch (Exception ex)
        {
            throw new Exception($"GetAllAsync: {ex.Message}");
        }
    }

    public async Task<SchoolDom> GetByIdAsync(int schoolId) {
        try {
            var school = await _context.Schools
                .FirstOrDefaultAsync(s => s.Id == schoolId);

            return _mapper.Map<SchoolDom>(school);
        } catch (Exception ex) {
            throw new Exception($"GetByIdAsync: {ex.Message}");
        }
    }

    public async Task<SchoolDom> AddAsync(SchoolDom schoolDom)
    {
        try
        {
            await _context.Schools.AddAsync(_mapper.Map<SchoolEF>(schoolDom));
            await _context.SaveChangesAsync();

            return schoolDom;
        }
        catch (Exception ex)
        {
            throw new Exception($"AddAsync: {ex.Message}");
        }
    }

    public async Task<SchoolDom> UpdateAsync(int schoolId, SchoolDom updatedSchool)
    {
        var existingSchool = await _context.Schools
        .FirstOrDefaultAsync(s => s.Id == schoolId);

        if (existingSchool == null)
        {
            throw new SchoolNotFoundException("School not found");
        }

        if (_mapper.Map<SchoolDom>(existingSchool).Equals(updatedSchool))
        {
            return _mapper.Map<SchoolDom>(existingSchool);
        }

        existingSchool.Name = updatedSchool.Name;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new SchoolException($"Something went wrong while updating your school: {ex.Message}");
        }

        return _mapper.Map<SchoolDom>(existingSchool);
    }

    public async Task<SchoolDom> UpsertAsync(int? schoolId, SchoolDom updatedSchool)
    {
        SchoolEF? currentSchool;
        if (schoolId.HasValue)
        {
            currentSchool = await GetSchoolById(schoolId.Value);
        }
        else
        {
            currentSchool = await GetSchoolByName(updatedSchool.Name);
        }

        if (currentSchool != null)
        {
            var currentSchoolDom = _mapper.Map<SchoolDom>(currentSchool);
            if (currentSchoolDom.Equals(updatedSchool))
            {
                return currentSchoolDom;
            }

            currentSchool.Name = updatedSchool.Name;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new SchoolException($"UpsertAsync: {ex.Message}");
            }

            return _mapper.Map<SchoolDom>(currentSchool);
        }

        return await AddAsync(updatedSchool);
    }

    public async Task DeleteAsync(int id)
    {
        var school = await _context.Schools
                .FirstOrDefaultAsync(s => s.Id == id);

        if (school == null)
        {
            throw new SchoolNotFoundException("School not found");
        }

        _context.Schools.Remove(school);
        await _context.SaveChangesAsync();
    }

    private async Task<SchoolEF?> GetSchoolById(int schoolId)
    {
        return await _context.Schools
        .FirstOrDefaultAsync(s => s.Id == schoolId);
    }

    private async Task<SchoolEF?> GetSchoolByName(string schoolName)
    {
        return await _context.Schools
       .FirstOrDefaultAsync(s => s.Name == schoolName);
    }
}