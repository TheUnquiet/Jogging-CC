using AutoMapper;
using Jogging.Domain.Configuration;
using Jogging.Domain.DomainManagers;
using Jogging.Domain.Exceptions;
using Jogging.Domain.Interfaces;
using Jogging.Domain.Interfaces.ServiceInterfaces;
using Jogging.Domain.Models;
using Jogging.Domain.Validators;
using Jogging.Rest.DTOs.AccountDtos.ConfirmDtos;
using Jogging.Rest.DTOs.AccountDtos.LoginDtos;
using Jogging.Rest.DTOs.AccountDtos.PasswordDtos;
using Jogging.Rest.DTOs.AccountDtos.SignUpDtos;
using Jogging.Rest.Utils;
using Jogging.Rest.DTOs.PersonDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Jogging.Rest.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBaseExtension {
        private readonly AuthManager _authManager;
        private readonly EmailManager _emailManager;
        private readonly PersonManager _personManager;
        private readonly IMapper _mapper;
        private readonly JwtConfiguration _configuration;
        private readonly ITokenBlacklistService _tokenBlacklistService;
        private readonly ICookieService _cookieService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AuthManager authManager, PersonManager personManager, IMapper mapper,
            IOptions<JwtConfiguration> configuration,
            ITokenBlacklistService tokenBlacklistService, ILogger<AuthController> logger, EmailManager emailManager, ICookieService cookieService) {
            _authManager = authManager;
            _personManager = personManager;
            _mapper = mapper;
            _configuration = configuration?.Value ?? throw new ArgumentNullException(nameof(configuration));
            _tokenBlacklistService = tokenBlacklistService;
            _logger = logger;
            _emailManager = emailManager;
            _cookieService = cookieService;
        }

        [HttpGet("verify-token")]
        [Authorize]
        public async Task<ActionResult<PersonResponseDTO>> VerifyToken() {
            try {
                var jwtPersonId = JwtTokenUtil.GetUserIdFromToken(HttpContext);
                var jwtPerson = await _personManager.GetByIdAsync(jwtPersonId);

                return Ok(new { Person = _mapper.Map<PersonResponseDTO>(jwtPerson) });
            } catch (Exception exception) {
                return InternalServerError(exception, _logger);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<PersonResponseDTO>> SignInAsync([FromBody] LogInRequestDTO person) {
            try {
                var success = await _authManager.SignInAsync(person.Email, person.Password);
                if (success != null) {
                    var jwtToken =
                        JwtTokenUtil.Generate(_configuration, success.Id, success.Profile.Role);

                    _cookieService.AddJwtCookie(Response, Request, jwtToken);

                    return Created(new { Person = _mapper.Map<PersonResponseDTO>(success), JwtToken = jwtToken });
                }

                return InternalServerError(new Exception("Something went wrong signing in."), _logger);
            } catch (Exception exception) {
                return InternalServerError(exception, _logger);
            }
        }

        [HttpPost("logout")]
        public ActionResult<bool> LogOutAsync() {
            try {
                var token = JwtTokenUtil.GetTokenFromCookie(HttpContext);
                _tokenBlacklistService.AddToBlacklist(token);

                _cookieService.RemoveJwtCookie(Response, Request);

                return Created(true);
            } catch (Exception exception) {
                return InternalServerError(exception, _logger);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> SignUpAsync([FromBody] SignUpRequestDTO signUpRequestDto) {
            try {
                var personDom = _mapper.Map<PersonDom>(signUpRequestDto.Person);
                personDom.Email = signUpRequestDto.Email;

                await _authManager.RegisterUserAsync(signUpRequestDto.Email, signUpRequestDto.Password, personDom, sendConfirmEmail: false);
                return Created();
            } catch (PasswordException exception) {
                return BadRequest(exception.Message);
            } catch (DuplicateEmailException exception) {
                return Conflict(exception.Message);
            } catch (Exception exception) {
                return InternalServerError(exception, _logger);
            }
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] PasswordChangeRequestDTO passwordChangeRequestDto) {
            try {
                passwordChangeRequestDto.UserId = JwtTokenUtil.GetGuIdFromToken(HttpContext);
                await _authManager.ChangePassword(_mapper.Map<PasswordChangeDom>(passwordChangeRequestDto));

                return Created();
            } catch (Exception exception) {
                return InternalServerError(exception, _logger);
            }
        }

        [HttpPost("request-password")]
        public async Task<IActionResult> RequestPasswordAsync([FromBody] EmailRequestDTO emailRequestDto) {
            try {
                string resetToken = await _authManager.ResetUserPasswordToken(emailRequestDto.Email);
                _emailManager.SendPasswordResetEmail(emailRequestDto.Email, resetToken);
                return Created();
            } catch (Exception exception) {
                return InternalServerError(exception, _logger);
            }
        }

        #region ConfirmEmail

        [HttpPost("request-confirm-mail")]
        public async Task<IActionResult> RequestConfirmMail([FromBody] EmailRequestDTO emailRequestDto) {
            try {
                AuthenticationValidator.ValidateEmailInput(emailRequestDto.Email);

                PersonDom person = await _personManager.GetByEmailAsync(emailRequestDto.Email);
                string confirmToken = await _authManager.ResetUserConfirmToken(emailRequestDto.Email);
                _emailManager.SendConfirmEmail(emailRequestDto.Email, confirmToken, person);
                return Created("Successfully requested new confirmation email");
            } catch (Exception exception) {
                return InternalServerError(exception, _logger);
            }
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmTokenDto confirmToken) {
            try {
                await _authManager.ConfirmEmailAsync(_mapper.Map<ConfirmTokenDom>(confirmToken));

                return Created("Email confirmed successfully");
            } catch (Exception exception) {
                return InternalServerError(exception, _logger);
            }
        }

        #endregion

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] PasswordResetRequestDTO passwordResetRequestDto) {
            try {
                await _authManager.ResetPasswordAsync(_mapper.Map<PasswordResetDom>(passwordResetRequestDto));

                return Created("Password reset successfully");
            } catch (Exception exception) {
                return InternalServerError(exception, _logger);
            }
        }

        [HttpPost("check-email")]
        public async Task<IActionResult> CheckDuplicateEmailAsync([FromBody] EmailRequestDTO emailRequestDto) {
            try {
                await _authManager.CheckDuplicateEmailAddressAsync(emailRequestDto.Email);

                return Created();
            } catch (DuplicateEmailException exception) {
                return Conflict(exception.Message);
            } catch (Exception exception) {
                return InternalServerError(exception, _logger);
            }
        }
    }
}
