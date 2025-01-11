using AutoMapper;
using Jogging.Domain.DomainManagers;
using Jogging.Domain.Exceptions;
using Jogging.Domain.Helpers;
using Jogging.Domain.Models;
using Jogging.Rest.DTOs.SchoolDtos;
using Jogging.Rest.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Jogging.Rest.Controllers {
#if ProducesConsumes
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
#endif
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolController : ControllerBaseExtension {
        #region Props

        private readonly SchoolManager _schoolManager;
        private readonly IMapper _mapper;
        private readonly ILogger<SchoolController> _logger;

        #endregion Props

        #region CTor

        public SchoolController(SchoolManager schoolManager, IMapper mapper, ILogger<SchoolController> logger) {
            _schoolManager = schoolManager;
            _mapper = mapper;
            _logger = logger;
        }

        #endregion CTor

        #region GET

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<PagedList<SchoolResponseDTO>>> GetAll(
            [FromQuery] QueryStringParameters parameters) {
            try {
                var schoolResponseDom = await _schoolManager.GetAll(parameters);

                ControllerUtil.AddPagination(schoolResponseDom, Response);

                return Ok(_mapper.Map<PagedList<SchoolDom>, PagedList<SchoolResponseDTO>>(schoolResponseDom));
            } catch (Exception ex) when (ex is SchoolNotFoundException) {
                return NotFound(ex.Message);
            } catch (Exception exception) {
                return InternalServerError(exception, _logger);
            }
        }

        [HttpGet("{schoolId:int}")]
        [Authorize]
        public async Task<ActionResult<SchoolDom>> Get(int schoolId) {
            try {
                var response = await _schoolManager.GetById(schoolId);

                if (response != null) {
                    return Ok(_mapper.Map<SchoolDom>(response));
                }

                return NotFound($"No school found with id {schoolId}.");
            } catch (Exception ex) when (ex is SchoolNotFoundException) {
                return NotFound(ex.Message);
            } catch (Exception exception) {
                return InternalServerError(exception, _logger);
            }
        }

        #endregion GET

        #region POST

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<SchoolDom>> Post([FromBody] SchoolRequestDTO schoolRequestDto) {
            try {
                var response = await _schoolManager.AddAsync(_mapper.Map<SchoolDom>(schoolRequestDto));

                if (response != null) {
                    return Created(_mapper.Map<SchoolResponseDTO>(response));
                }

                return InternalServerError(new Exception("Failed to add school. Check your input data."), _logger);
            } catch (Exception exception) {
                return InternalServerError(exception, _logger);
            }
        }

        #endregion POST

        #region PUT

        [HttpPut("{schoolId:int}")]
        [Authorize]
        public async Task<ActionResult<SchoolResponseDTO>> Put(int schoolId, [FromBody] SchoolRequestDTO schoolRequestDto) {
            try {
                var response = await _schoolManager.UpdateAsync(schoolId, _mapper.Map<SchoolDom>(schoolRequestDto));

                if (response != null) {
                    return Created(_mapper.Map<SchoolResponseDTO>(response));
                }

                return InternalServerError(new Exception("Failed to update school. Check your input data."), _logger);
            } catch (Exception exception) {
                return InternalServerError(exception, _logger);
            }
        }

        #endregion PUT

        #region DELETE

        [HttpDelete("{schoolId:int}")]
        [Authorize]
        public async Task<ActionResult> Delete(int schoolId) {
            try {
                await _schoolManager.DeleteAsync(schoolId);

                return Ok();
            } catch (Exception exception) {
                return InternalServerError(exception, _logger);
            }
        }

        #endregion DELETE
    }
}