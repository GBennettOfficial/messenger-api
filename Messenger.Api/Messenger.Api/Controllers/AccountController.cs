using FluentValidation;
using Messenger.Api.Areas.Cryptography.Abstractions;
using Messenger.Api.Areas.Cryptography.Models;
using Messenger.Api.Areas.Data.Abstractions;
using Messenger.Api.Areas.Data.Models;
using Messenger.Api.Utilities;
using Messenger.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Messenger.Api.Controllers
{
    [Route("Account")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly IValidator<RegisterDto> _registerDtoValidator;
        private readonly IValidator<UsernameLoginDto> _usernameLoginDtoValidator;
        private readonly IPasswordHasher _passwordHashser;
        private readonly IPasswordChecker _passwordChecker;
        private readonly IUserReader _userReader;
        private readonly IUserWriter _userWriter;
        private readonly ISocialConnectionsReader _socialConnectionsReader;
        private readonly IJsonWebTokenProvider _jsonWebTokenProvider;
        private readonly RandomUserCodeGenerator _randomUserCodeGenerator;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IValidator<RegisterDto> registerDtoValidator,
                                 IValidator<UsernameLoginDto> usernameLoginDtoValidator,
                                 IPasswordHasher passwordHashser,
                                 IPasswordChecker passwordChecker,
                                 IUserReader userReader,
                                 IUserWriter userWriter,
                                 ISocialConnectionsReader socialConnectionsReader,
                                 IJsonWebTokenProvider jsonWebTokenProvider,
                                 RandomUserCodeGenerator randomUserCodeGenerator,
                                 ILogger<AccountController> logger)
        {
            _registerDtoValidator = registerDtoValidator;
            _usernameLoginDtoValidator = usernameLoginDtoValidator;
            _passwordHashser = passwordHashser;
            _passwordChecker = passwordChecker;
            _userReader = userReader;
            _userWriter = userWriter;
            _socialConnectionsReader = socialConnectionsReader;
            _jsonWebTokenProvider = jsonWebTokenProvider;
            _randomUserCodeGenerator = randomUserCodeGenerator;
            _logger = logger;
        }

        
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var validationResult = await _registerDtoValidator.ValidateAsync(registerDto);
            if (validationResult.IsValid == false)
                return BadRequest(string.Join("\n", validationResult.Errors.Select(x => x.ErrorMessage)));

            int c = 0;
            bool userCodeExists = true;
            string userCode = "";
            while (userCodeExists)
            {
                c++;
                if (c > 10)
                    return StatusCode(500, "Something went wrong");
                userCode = _randomUserCodeGenerator.GetRandomUserCode();
                var result = await _userReader.SearchByUserCode(userCode);
                if (result.Code == ResultCode.NotFound)
                    userCodeExists = false;
            }

            HashedPassword hashedPassword = _passwordHashser.HashPassword(registerDto.Password);

            User user = new(0, registerDto.Username, userCode, registerDto.FirstName, registerDto.LastName, registerDto.Email, registerDto.Phone, hashedPassword.Hash, hashedPassword.Salt, registerDto.Picture);

            Result<User> createUserResult = await _userWriter.Create(user);
            if (createUserResult.Code != ResultCode.Success || createUserResult.Value is null)
                return StatusCode(500, "Something went wrong");

            JsonWebTokenDto jwt = _jsonWebTokenProvider.CreateJsonWebToken(createUserResult.Value!);

            Result<SocialConnectionsDto> socialConnectionsResult = await _socialConnectionsReader.Read(registerDto.Username);
            if (socialConnectionsResult.Code != ResultCode.Success || socialConnectionsResult.Value is null)
                return StatusCode(500, "Something went wrong");

            LoginResponseDto loginResponseDto = new(jwt, socialConnectionsResult.Value!);


            _logger.LogInformation("User {Username} registered", registerDto.Username);

            return Ok(loginResponseDto);
        }

        [HttpPost]
        [Route("UsernameLogin")]
        public async Task<IActionResult> UsernameLogin([FromBody] UsernameLoginDto usernameLoginDto)
        {
            Result<User> findUserResult = await _userReader.SearchByUsername(usernameLoginDto.Username);

            if (findUserResult.Code == ResultCode.NotFound)
                return NotFound("Username doesn't exist");

            if (findUserResult.Code != ResultCode.Success || findUserResult.Value is null)
                return StatusCode(500, "Something went wrong");

            HashedPassword hashedPassword = new(findUserResult.Value.PasswordHash, findUserResult.Value.PasswordSalt);

            bool passwordIsValid = _passwordChecker.CheckPassword(usernameLoginDto.Password, hashedPassword);

            if (passwordIsValid == false)
                return BadRequest("Invalid username or password");

            User newUser = findUserResult.Value with { LastLoginDate = DateTime.UtcNow };
            var updateResult = await _userWriter.Update(newUser);
            if (updateResult.Code != ResultCode.Success)
                return StatusCode(500, "Something went wrong");

            JsonWebTokenDto jwt = _jsonWebTokenProvider.CreateJsonWebToken(findUserResult.Value!);

            Result<SocialConnectionsDto> socialConnectionsResult = await _socialConnectionsReader.Read(usernameLoginDto.Username);
            if (socialConnectionsResult.Code != ResultCode.Success || socialConnectionsResult.Value is null)
                return StatusCode(500, "Something went wrong");

            LoginResponseDto loginResponseDto = new(jwt, socialConnectionsResult.Value!);


            _logger.LogInformation("User {Username} logged in", usernameLoginDto.Username);

            return Ok(loginResponseDto);
        }

        [Authorize]
        [HttpGet("RefreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            string username = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            Result<User> findUserResult = await _userReader.SearchByUsername(username);
            if (findUserResult.Code != ResultCode.Success)
                return StatusCode(500, "Something went wrong");

            JsonWebTokenDto jwt = _jsonWebTokenProvider.CreateJsonWebToken(findUserResult.Value!);

            Result<SocialConnectionsDto> socialConnectionsResult = await _socialConnectionsReader.Read(username);
            if (socialConnectionsResult.Code != ResultCode.Success || socialConnectionsResult.Value is null)
                return StatusCode(500, "Something went wrong");

            LoginResponseDto loginResponseDto = new(jwt, socialConnectionsResult.Value!);

            _logger.LogInformation("User {Username} refreshed token", username);

            return Ok(loginResponseDto);
        }


    }
}
