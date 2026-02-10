using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MovieReview.Models.DTOs;
using MovieReview.Models.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MovieReview.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<User> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<AuthResultDto> RegisterAsync(RegisterDto dto)
        {
            User? existingUser = await _userManager.FindByNameAsync(dto.Username);
            if (existingUser is not null)
                return new AuthResultDto { Success = false, Message = "Username already exists" };

            User user = new User
            {
                UserName = dto.Username,
                Email = dto.Email
            };

            IdentityResult result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return new AuthResultDto { Success = false, Message = string.Join(", ", result.Errors.Select(e => e.Description)) };

            string token = GenerateJwtToken(user);
            return new AuthResultDto { Success = true, Token = token, Message = "Registration successful" };
        }

        public async Task<AuthResultDto> LoginAsync(LoginDto dto)
        {
            User? user = await _userManager.FindByNameAsync(dto.Username);
            if (user == null)
                return new AuthResultDto { Success = false, Message = "Invalid username or password" };

            bool passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!passwordValid)
                return new AuthResultDto { Success = false, Message = "Invalid username or password" };

            string token = GenerateJwtToken(user);
            return new AuthResultDto { Success = true, Token = token, Message = "Login successful" };
        }

        private string GenerateJwtToken(User user)
        {
            IConfigurationSection jwtSettings = _configuration.GetSection("Jwt");
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured")));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            Claim[] claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), 
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
