using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ExpenseTrackerNewAPI.Core.Entities;
using ExpenseTrackerNewAPI.Core.Interfaces;

namespace ExpenseTrackerNewAPI.Application.Services
{
    public class AuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthenticationService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<string> GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiryMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }

        public async Task<bool> ValidateRefreshToken(User user, string refreshToken)
        {
            return user.RefreshToken == refreshToken && user.RefreshTokenExpiryTime > DateTime.Now;
        }

        public async Task UpdateUserRefreshToken(int userId, string refreshToken)
        {
            var expiryTime = DateTime.Now.AddDays(Convert.ToDouble(_configuration["Jwt:RefreshTokenExpiryDays"]));
            await _userRepository.UpdateRefreshTokenAsync(userId, refreshToken, expiryTime);
        }
    }
} 