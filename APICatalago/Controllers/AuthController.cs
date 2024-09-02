using APICatalago.DTOS;
using APICatalago.Models;
using APICatalago.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace APICatalago.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthController> _logger;
        public AuthController(ITokenService tokenService,
               UserManager<ApplicationUser> userManager,
               RoleManager<IdentityRole> roleManager,
               IConfiguration configuration, ILogger<AuthController> logger)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _roleManager = roleManager;
            _config = configuration;
            _logger = logger;

        }

        [HttpPost]
        [Route("CreateRole")]
        public async Task<ActionResult> CreateRole(string role)
        {
            var roleExists = await _roleManager.RoleExistsAsync(role);
            if (!roleExists)
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(role));
                if (roleResult.Succeeded)
                {
                    _logger.LogInformation(1, "Roles Added");
                    return StatusCode(StatusCodes.Status201Created, new ResponseDTO { Status = "Success", Message = $"Role {role} created successfully!" });
                }
                else
                {
                    _logger.LogInformation(2, "Error Adding Roles");
                    return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDTO { Status = "Error", Message = "Error creating role! Please check role details and try again." });
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDTO { Status = "Error", Message = "Role already exists!" });
            }
        }

        [HttpPost]
        [Route("AddUserToRole")]
        public async Task<ActionResult> AddUserToRole(string email, string rolename)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.AddToRoleAsync(user, rolename);
                if (result.Succeeded)
                {
                    _logger.LogInformation(1, $"User {user.Email} added to the {rolename} role");
                    return StatusCode(StatusCodes.Status201Created, new ResponseDTO { Status = "Success", Message = $"User {user.Email} added to the {rolename} role" });
                }
                else
                {
                    _logger.LogInformation(2, $"Error unable to add user {user.Email} to the {rolename} role");
                    return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDTO { Status = "Error", Message = $"Error unable to add user {user.Email} to the {rolename} role" });
                }
            }
            return BadRequest(new { error = "unable to find user" });
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login([FromBody] LoginModelDTO loginModel)
        {
            var user = await _userManager.FindByNameAsync(loginModel.UserName!);
            if (user is not null && await _userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email,user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }
                var token = _tokenService.GenerateAccessToken(authClaims, _config);
                var refreshToken = _tokenService.GenerateRefreshToken();
                _ = int.TryParse(_config["Jwt:RefreshTokenValidityInMinutes"], out int refreshTokenValidityInMinutes);
                user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(refreshTokenValidityInMinutes);
                user.RefreshToken = refreshToken;
                await _userManager.UpdateAsync(user);
                return Ok(new
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    RefreshToken = refreshToken,
                    Expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }

        [HttpPost]
        [Route("Register")]
        public async Task<ActionResult> Register([FromBody] RegisterModelDTO registerModel)
        {
            var userExists = await _userManager.FindByNameAsync(registerModel.UserName!);

            if (userExists != null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDTO { Status = "Error", Message = "User already exists!" });
            }
            ApplicationUser user = new ApplicationUser()
            {
                Email = registerModel.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerModel.UserName
            };
            var result = await _userManager.CreateAsync(user, registerModel.Password);

            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDTO { Status = "Error", Message = "User creation failed! Please check user details and try again." });
            }
            return Ok(new ResponseDTO { Status = "Success", Message = "User created successfully!" });
        }

        [HttpPost]
        [Route("refresh-token")]
        public async Task<ActionResult> RefreshToken(TokenModelDTO tokenModel)
        {
            if (tokenModel is null)
            {
                return BadRequest(new ResponseDTO { Status = "Error", Message = "Invalid client request" });
            }
            string? acessToken = tokenModel.AcessToken ?? throw new ArgumentException(nameof(tokenModel.AcessToken));
            string? refreshToken = tokenModel.RefreshToken ?? throw new ArgumentException(nameof(tokenModel.RefreshToken));

            var main = _tokenService.GetMainFromExpiredToken(acessToken!, _config);
            if (main == null)
            {

                return BadRequest("Invalid acesss token/refresh token");
            }
            var username = main.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);
            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return BadRequest("Invalid acesss token/refresh token");
            }
            var newAcessToken = _tokenService.GenerateAccessToken(main.Claims.ToList(), _config);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);
            return new ObjectResult(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(newAcessToken),
                refreshToken = newRefreshToken
            });
        }
        [Authorize]
        [HttpPost]
        [Route("revoke/{userName}")]
        public async Task<ActionResult> Revoke(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return BadRequest("Invalid acesss token/refresh token");
            }
            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);
            return NoContent();
        }
    }
}
