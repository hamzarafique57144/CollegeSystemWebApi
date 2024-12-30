using CollegeAppWebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CollegeAppWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly APIResponse _apiResponse;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
            _apiResponse = new APIResponse(); // Initialize APIResponse
        }

        [HttpPost]
        public IActionResult Login(LoginDTO model)
        {
            if (!ModelState.IsValid)
            {
                _apiResponse.Status = false;
                _apiResponse.Error = new List<string> { "Invalid input data." };
                return BadRequest(_apiResponse);
            }

            // Dummy validation for username and password
            if (model.UserName == "Hamza" && model.Password == "123qwe")
            {
                // Validate JWT settings
                var secretKey = _configuration.GetValue<string>("JwtSettings:SecretKey");
                var audience = _configuration.GetValue<string>("JwtSettings:Audience");
                var issuer = _configuration.GetValue<string>("JwtSettings:Issuer");

                if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(audience) || string.IsNullOrEmpty(issuer))
                {
                    _apiResponse.Status = false;
                    _apiResponse.Error = new List<string> { "JWT configuration is missing or incomplete." };
                    return StatusCode(StatusCodes.Status500InternalServerError, _apiResponse);
                }

                var key = Encoding.ASCII.GetBytes(secretKey);
                var tokenHandler = new JwtSecurityTokenHandler();

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, model.UserName),
                        new Claim(ClaimTypes.Role, "Admin")
                    }),
                    Expires = DateTime.UtcNow.AddHours(4),
                    Audience = audience,
                    Issuer = issuer,
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha512Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);

                _apiResponse.Status = true;
                _apiResponse.Data = new LoginResponseDTO
                {
                    UserName = model.UserName,
                    Token = tokenHandler.WriteToken(token),
                    Expiration = token.ValidTo
                };
                return Ok(_apiResponse);
            }

            // Invalid credentials
            _apiResponse.Status = false;
            _apiResponse.Error = new List<string> { "Invalid credentials." };
            return Unauthorized(_apiResponse);
        }
    }
}
