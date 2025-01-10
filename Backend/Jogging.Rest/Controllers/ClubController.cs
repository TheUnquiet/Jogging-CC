using AutoMapper;
using Jogging.Domain.DomainManagers;
using Jogging.Domain.Exceptions;
using Jogging.Domain.Helpers;
using Jogging.Domain.Models;
using Jogging.Rest.DTOs.ClubDtos;
using Jogging.Rest.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Jogging.Rest.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class ClubController : ControllerBaseExtension {

        [HttpGet("{clubId:int}/members")]
        [Authorize]
        public async Task<ActionResult<ClubResponseDTO>> GetClubByIdWithMembers(int clubId) {
            try {
                // Haal de club op inclusief leden
                var club = await _clubManager.GetByIdWithMembersAsync(clubId);

                if (club == null) {
                    return NotFound($"Club with id {clubId} was not found.");
                }

                // Map naar ClubResponseDto inclusief leden
                return Ok(_mapper.Map<ClubResponseDTO>(club));
            } catch (Exception exception) {
                _logger.LogError(exception, "An error occurred in GetClubByIdWithMembers.");
                return InternalServerError(exception, _logger);
            }
        }


        #region Props

        private readonly ClubManager _clubManager;
        private readonly IMapper _mapper;
        private readonly ILogger<ClubController> _logger;

        #endregion Props

        #region CTor

        public ClubController(ClubManager clubManager, IMapper mapper, ILogger<ClubController> logger) {
            _clubManager = clubManager;
            _mapper = mapper;
            _logger = logger;
        }

        #endregion CTor

        #region GET

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<PagedList<ClubResponseDTO>>> GetAll([FromQuery] QueryStringParameters parameters) {
            try {
                var clubs = await _clubManager.GetAllAsync(parameters);
                ControllerUtil.AddPagination(clubs, Response);

                return Ok(_mapper.Map<PagedList<ClubDom>, PagedList<ClubResponseDTO>>(clubs));
            } catch (Exception exception) {
                _logger.LogError(exception, "An error occurred in GetAll.");
                return InternalServerError(exception, _logger);
            }
        }

        [HttpGet("{clubId:int}")]
        [Authorize]
        public async Task<ActionResult<ClubResponseDTO>> Get(int clubId) {
            try {
                var club = await _clubManager.GetByIdAsync(clubId);
                if (club == null) {
                    return NotFound($"Club with id {clubId} was not found.");
                }

                return Ok(_mapper.Map<ClubResponseDTO>(club));
            } catch (Exception exception) {
                _logger.LogError(exception, "An error occurred in Get.");
                return InternalServerError(exception, _logger);
            }
        }

        #endregion GET

        #region POST

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ClubResponseDTO>> CreateClub([FromBody] ClubRequestDTO clubRequest) {
            try {
                var createdClub = await _clubManager.CreateAsync(_mapper.Map<ClubDom>(clubRequest));
                return CreatedAtAction(nameof(Get), new { clubId = createdClub.Id }, _mapper.Map<ClubResponseDTO>(createdClub));
            } catch (Exception exception) {
                _logger.LogError(exception, "An error occurred in Create.");
                return InternalServerError(exception, _logger);
            }
        }

        #endregion POST

        #region PUT

        [HttpPut("{clubId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ClubResponseDTO>> UpdateClub(int clubId, [FromBody] ClubRequestDTO clubRequest) {
            try {
                var updatedClub = await _clubManager.UpdateAsync(clubId, _mapper.Map<ClubDom>(clubRequest));
                return Ok(_mapper.Map<ClubResponseDTO>(updatedClub));
            } catch (Exception exception) {
                _logger.LogError(exception, "An error occurred in Update.");
                return InternalServerError(exception, _logger);
            }
        }

        #endregion PUT

        #region DELETE

        [HttpDelete("{clubId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteClub(int clubId) {
            try {
                await _clubManager.DeleteAsync(clubId);
                return Ok("Club deleted successfully.");
            } catch (Exception exception) {
                _logger.LogError(exception, "An error occurred in Delete.");
                return InternalServerError(exception, _logger);
            }
        }

        #endregion DELETE
    }
}
