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

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult Login(LoginDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors
            }

            // Dummy validation for username and password
            if (model.UserName == "Hamza" && model.Password == "123qwe")
            {
                // Retrieve required JWT settings
                var secretKey = _configuration.GetValue<string>("JwtSettings:SecretKey");
                var audience = _configuration.GetValue<string>("JwtSettings:Audience");
                var issuer = _configuration.GetValue<string>("JwtSettings:Issuer");

                if (string.IsNullOrEmpty(secretKey))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Secret key is not configured.");
                }

                var key = Encoding.ASCII.GetBytes(secretKey);
                var tokenHandler = new JwtSecurityTokenHandler();

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, model.UserName), // Add Name claim
                        new Claim(ClaimTypes.Role, "Admin")          // Add Role claim
                    }),
                    Expires = DateTime.UtcNow.AddHours(4), // Token expiration
                    Audience = audience,                  // Add Audience claim
                    Issuer = issuer,                      // Add Issuer claim
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha512Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);

                return Ok(new LoginResponseDTO
                {
                    UserName = model.UserName,
                    Token = tokenHandler.WriteToken(token),
                    Expiration = token.ValidTo
                });
            }

            // Generic error message for invalid credentials
            return Unauthorized("Invalid credentials.");
        }
    }
}
