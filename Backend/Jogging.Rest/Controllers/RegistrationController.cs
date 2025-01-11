using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Jogging.Domain.DomainManagers;
using Jogging.Domain.Exceptions;
using Jogging.Domain.Helpers;
using Jogging.Domain.Models;
using Jogging.Rest.Utils;
using Jogging.Rest.DTOs.RegistrationDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Postgrest.Exceptions;

namespace Jogging.Rest.Controllers;

#if ProducesConsumes
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
#endif

[Route("api/[controller]")]
[ApiController]
public class RegistrationController : ControllerBaseExtension
{
    private readonly RegistrationManager _registrationManager;
    private readonly IMapper _mapper;
    private readonly ILogger<RegistrationController> _logger;

    public RegistrationController(RegistrationManager registrationManager, IMapper mapper, ILogger<RegistrationController> logger)
    {
        _registrationManager = registrationManager;
        _mapper = mapper;
        _logger = logger;
    }

    #region GET

    [HttpGet("private")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<RegistrationResponseDTO>>> GetAllRegistrationsAdmin(
        [FromQuery] int? personId,
        [FromQuery] int? competitionId,
        [FromQuery] string? searchValue,
        [FromQuery] QueryStringParameters parameters,
        [FromQuery] bool withRunNumber = false)
    {
        try
        {
            var registrations = await _registrationManager.GetRegistrationsAsync(personId, competitionId, searchValue, withRunNumber, parameters);
            ControllerUtil.AddPagination(registrations, Response);
            return Ok(_mapper.Map<PagedList<RegistrationDom>, PagedList<RegistrationResponseDTO>>(registrations));
        }
        catch (RegistrationNotFoundException)
        {
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<PagedList<RegistrationResponseDTO>>> GetPersonRegistrations(
        [FromQuery] QueryStringParameters parameters,
        [FromQuery] bool withRunNumber = false)
    {
        try
        {
            int personId = JwtTokenUtil.GetUserIdFromToken(HttpContext);
            var registrations = await _registrationManager.GetPersonRegistrations(personId, parameters, withRunNumber);
            ControllerUtil.AddPagination(registrations, Response);
            return Ok(_mapper.Map<PagedList<RegistrationDom>, PagedList<RegistrationResponseDTO>>(registrations));
        }
        catch (RegistrationNotFoundException)
        {
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    #endregion

    #region POST

    [HttpPost("private")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<RegistrationResponseDTO>> AdminRegistration([FromBody] RegistrationRequestDTO registrationRequest)
    {
        try
        {
            RegistrationDom registrationDom;
            if (registrationRequest.PersonId != -1)
            {
                registrationDom = await _registrationManager.SignInExistingPersonToContestAsync(registrationRequest.CompetitionId, registrationRequest.PersonId,
                    registrationRequest.DistanceName);
            }
            else if (!string.IsNullOrWhiteSpace(registrationRequest.Email) &&  registrationRequest.Person != null)
            {
                registrationDom = await _registrationManager.SignInToContestWithEmailAsync(
                    registrationRequest.CompetitionId,
                    _mapper.Map<PersonDom>(registrationRequest.Person),
                    registrationRequest.Email,
                    registrationRequest.DistanceName);
            }
            else if (registrationRequest.Person != null)
            {
                registrationDom = await _registrationManager.SignInNewPersonToContestAsync(
                    registrationRequest.CompetitionId,
                    _mapper.Map<PersonDom>(registrationRequest.Person),
                    registrationRequest.DistanceName);
            }
            else
            {
                return InternalServerError(new Exception("Something went wrong this registration."), _logger);
            }

            return Created(_mapper.Map<RegistrationResponseDTO>(registrationDom));
        }
        catch (Exception ex) when (ex is DuplicateEmailException or RegistrationAlreadyExistsException)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            return InternalServerError(ex, _logger);
        }
    }

    [HttpPost]
    public async Task<ActionResult<RegistrationResponseDTO>> RegisterToContestAsync([FromBody] RegistrationRequestDTO registrationRequest)
    {
        try
        {
            RegistrationDom registrationDom;
            int jwtPersonId = JwtTokenUtil.GetUserIdFromToken(HttpContext, false);

            if (jwtPersonId != -1)
            {
                registrationDom = await _registrationManager.SignInExistingPersonToContestAsync(registrationRequest.CompetitionId, jwtPersonId,
                    registrationRequest.DistanceName);
            }
            else if (!string.IsNullOrWhiteSpace(registrationRequest.Email) && registrationRequest.Person != null)
            {
                registrationDom = await _registrationManager.SignInToContestWithEmailAsync(
                    registrationRequest.CompetitionId,
                    _mapper.Map<PersonDom>(registrationRequest.Person),
                    registrationRequest.Email,
                    registrationRequest.DistanceName);
            }
            else if (registrationRequest.Person != null)
            {
                registrationDom = await _registrationManager.SignInNewPersonToContestAsync(
                    registrationRequest.CompetitionId,
                    _mapper.Map<PersonDom>(registrationRequest.Person),
                    registrationRequest.DistanceName);
            }
            else
            {
                return InternalServerError(new Exception("Something went wrong this registration."), _logger);
            }

            return Created(_mapper.Map<RegistrationResponseDTO>(registrationDom));
        }
        catch (Exception ex) when (ex is DuplicateEmailException or RegistrationAlreadyExistsException)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            return InternalServerError(ex, _logger);
        }
    }

    #endregion

    #region DELETE

    [HttpDelete("{competitionId}", Name = nameof(DeleteUserRegistration))]
    [Authorize]
    public async Task<ActionResult<string>> DeleteUserRegistration(int competitionId)
    {
        try
        {
            int jwtPersonId = JwtTokenUtil.GetUserIdFromToken(HttpContext);
            await _registrationManager.DeleteUserRegistration(jwtPersonId, competitionId);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("private/{registrationId}", Name = nameof(DeleteRegistrationAsync))]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<string>> DeleteRegistrationAsync(int registrationId) {
        try {
            await _registrationManager.DeleteUserRegistration(registrationId);
            return Ok("Registration successfully deleted.");
        } catch (RegistrationNotFoundException) {
            return NotFound("The registration with the specified ID was not found.");
        } catch (Exception ex) {
            _logger.LogError($"Error occurred while deleting registration ID {registrationId}: {ex.Message}");
            return InternalServerError(ex, _logger);
        }
    }

    #endregion

    #region PUT

    [HttpPut("paid/{registrationId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<string>> UpdateRegistrationPaidAsync(
        int registrationId,
        [FromBody] RegistrationModifyPaidDTO registrationToModify)
    {
        try
        {
            await _registrationManager.UpdatePaidAsync(registrationId, _mapper.Map<RegistrationDom>(registrationToModify));
            return Created("Updated successfully");
        }
        catch (PostgrestException ex) when (ex.Message.Contains("23505"))
        {
            return Conflict("This run number is already used for this competition");
        }
        catch (Exception ex)
        {
            return InternalServerError(ex, _logger);
        }
    }

    [HttpPut("runnumber/{registrationId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<string>> UpdateRegistrationRunNumberAsync(
        int registrationId,
        [FromBody] RegistrationModifyRunNumberDTO registrationToModify)
    {
        try
        {
            await _registrationManager.UpdateRunNumberAsync(registrationId, _mapper.Map<RegistrationDom>(registrationToModify));
            return Created("Updated successfully");
        }
        catch (PostgrestException ex) when (ex.Message.Contains("23505"))
        {
            return Conflict("This run number is already used for this competition");
        }
        catch (Exception ex)
        {
            return InternalServerError(ex, _logger);
        }
    }

    [HttpPut("competitionpercategory/{registrationId:int}")]
    [Authorize]
    public async Task<ActionResult<string>> UpdateCompetitionPerCategoryAsync(
        int registrationId,
        [FromBody] RegistrationModifyCompetitionPerCategoryDTO registrationToModify)
    {
        try
        {
            int personId = JwtTokenUtil.GetUserIdFromToken(HttpContext);
            await _registrationManager.UpdateCompetitionPerCategoryAsync(
                registrationId,
                personId,
                _mapper.Map<CompetitionPerCategoryDom>(registrationToModify));
            return Created("Updated successfully");
        }
        catch (PostgrestException ex) when (ex.Message.Contains("23505"))
        {
            return Conflict("This run number is already used for this competition");
        }
        catch (Exception ex)
        {
            return InternalServerError(ex, _logger);
        }
    }

    [HttpPut("private/competitionpercategory/{registrationId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<string>> AdminUpdateCompetitionPerCategoryAsync(
        int registrationId,
        [FromQuery, Required] int personId,
        [FromBody] RegistrationModifyCompetitionPerCategoryDTO registrationToModify)
    {
        try
        {
            await _registrationManager.UpdateCompetitionPerCategoryAsync(
                registrationId,
                personId,
                _mapper.Map<CompetitionPerCategoryDom>(registrationToModify));
            return Created("Updated successfully");
        }
        catch (PostgrestException ex) when (ex.Message.Contains("23505"))
        {
            return Conflict("This run number is already used for this competition");
        }
        catch (Exception ex)
        {
            return InternalServerError(ex, _logger);
        }
    }

    #endregion
}