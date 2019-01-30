using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Roulette.Domain.Entities;
using Roulette.Persistance;
using Roulette.Services.Responses;
using Roulette.Services.Users.Abstracts;
using Roulette.Services.Users.Models.Read;
using Roulette.Services.Users.Models.Write;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Roulette.Services.Users
{
    public class UserService : IUserService
    {
        #region PrivateFields

        private readonly RouletteDbContext _db;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        #endregion

        #region Constuctor

        public UserService(
            RouletteDbContext db,
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            IConfiguration configuration)
        {
            _db = db;
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
        }

        #endregion

        #region GetMethods

        public async Task<Response<UserPreviewDto>> GetUserAsync(Guid? id)
        {
            var response = new Response<UserPreviewDto>();

            if (id == null)
            {
                response.SetBadRequestStatusCode();
                return response;
            }

            try
            {
                var userDto = UserPreviewDto.Projection.Compile().Invoke(await _db.Set<User>().SingleOrDefaultAsync(u => u.Id == id));
                if (userDto == null)
                {
                    response.SetNotFountStatusCode();
                    response.SetErrorMessages("User not found.");
                    return response;
                }

                response.SetSuccess();
                response.SetOkStatusCode();
                response.SetModel(userDto);

                return response;
            }
            catch (Exception ex)
            {
                response.SetInternalServerErrorStatusCode();
                response.SetErrorMessages(ex.Message);
                return response;
            }
        }

        #endregion

        #region PostMethods

        public async Task<Response<UserPreviewDto>> LoginAsync(LoginUserDto model)
        {
            var response = new Response<UserPreviewDto>();

            if (model == null)
            {
                response.SetBadRequestStatusCode();
                return response;
            }

            try
            {
                var signInResult = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
                if (!signInResult.Succeeded)
                {
                    response.SetUnauthorizedStatusCode();
                    response.SetErrorMessages("Incorrect 'Username' or 'Passowrd'.");
                    return response;
                }

                var appUser = await _userManager.Users.SingleOrDefaultAsync(u => u.UserName == model.UserName);
                if (appUser == null)
                {
                    response.SetNotFountStatusCode();
                    response.SetErrorMessages("User not found.");
                    return response;
                }

                return GenerateJwtToken(appUser);

            }
            catch (Exception ex)
            {
                response.SetInternalServerErrorStatusCode();
                response.SetErrorMessages(ex.Message);
                return response;
            }
        }

        public async Task<Response<UserPreviewDto>> RegisterAsync(RegisterUserDto model)
        {
            var response = new Response<UserPreviewDto>();

            if (model == null)
            {
                response.SetBadRequestStatusCode();
                return response;
            }

            try
            {
                var appUser = RegisterUserDto.CreateEntity.Compile().Invoke(model);
                if (appUser == null)
                {
                    response.SetAcceptedStatusCode();
                    response.SetErrorMessages("Registering user failed.");
                    return response;
                }

                appUser.RefreshToken = GenerateRefreshToken();

                var regResult = await _userManager.CreateAsync(appUser, model.Password);
                if (!regResult.Succeeded)
                {
                    response.SetUnauthorizedStatusCode();
                    response.SetErrorMessages(regResult.Errors.Select(i => i.Description).ToArray());
                    return response;
                }

                return GenerateJwtToken(appUser);

            }
            catch (Exception ex)
            {
                response.SetInternalServerErrorStatusCode();
                response.SetErrorMessages(ex.Message);
                return response;
            }
        }

        #endregion

        #region PutMethods

        public async Task<Response<UserPreviewDto>> UpdateAsync(UpdateUserDto model)
        {
            var response = new Response<UserPreviewDto>();

            if (model == null)
            {
                response.SetBadRequestStatusCode();
                return response;
            }

            try
            {
                var userEntity = await _db.Set<User>().FindAsync(model.Id);
                if (userEntity == null)
                {
                    response.SetNotFountStatusCode();
                    response.SetErrorMessages("User not found.");
                    return response;
                }

                var updatedUserEntity = UpdateUserDto.UpdateEntity(model, userEntity);
                if (updatedUserEntity == null)
                {
                    response.SetAcceptedStatusCode();
                    response.SetErrorMessages("Updating user failed.");
                    return response;
                }

                _db.Set<User>().Update(updatedUserEntity);
                await _db.SaveChangesAsync();

                var userDto = UserPreviewDto.Projection.Compile().Invoke(updatedUserEntity);

                response.SetSuccess();
                response.SetOkStatusCode();
                response.SetModel(userDto);

                return response;

            }
            catch (Exception ex)
            {
                response.SetInternalServerErrorStatusCode();
                response.SetErrorMessages(ex.Message);
                return response;
            }
        }

        public async Task<Response<UserPreviewDto>> RefreshTokenAsync(string token, string refreshToken)
        {
            var response = new Response<UserPreviewDto>();

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(refreshToken))
            {
                response.SetBadRequestStatusCode();
                return response;
            }

            try
            {
                var principal = GetPrincipalFromExpiredToken(token);

                var userIdString = principal.Claims.SingleOrDefault(i => i.Type == JwtRegisteredClaimNames.Sub)?.Value;
                Guid.TryParse(userIdString, out Guid userId);
                
                var user = await _db.Set<User>().SingleOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    response.SetUnauthorizedStatusCode();
                    response.SetErrorMessages("User not found.");
                    return response;
                }

                if (!user.RefreshToken.Equals(refreshToken))
                {
                    response.SetUnauthorizedStatusCode();
                    response.SetErrorMessages("Invalid refresh token.");
                    return response;
                }

                var newJwtToken = GenerateToken(principal.Claims);
                var newRefreshToken = GenerateRefreshToken();

                user.RefreshToken = newRefreshToken;

                _db.Set<User>().Update(user);
                await _db.SaveChangesAsync();

                var userDto = UserPreviewDto.Projection.Compile().Invoke(user);
                userDto.JwtToken = newJwtToken;

                response.SetSuccess();
                response.SetOkStatusCode();
                response.SetModel(userDto);

                return response;
            }
            catch (Exception ex)
            {
                response.SetInternalServerErrorStatusCode();
                response.SetErrorMessages(ex.Message);
                return response;
            }   
        }

        #endregion

        #region HelperMethods

        private Response<UserPreviewDto> GenerateJwtToken(User user)
        {
            var response = new Response<UserPreviewDto>();

            try
            {
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Email, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var jwtToken = GenerateToken(claims);
                if (string.IsNullOrEmpty(jwtToken))
                {
                    response.SetUnauthorizedStatusCode();
                    response.SetErrorMessages("Generating Jwt Token failed.");
                    return response;
                }

                var userDto = UserPreviewDto.Projection.Compile().Invoke(user);
                userDto.JwtToken = jwtToken;

                response.SetSuccess();
                response.SetOkStatusCode();
                response.SetModel(userDto);

                return response;
            }
            catch (Exception ex)
            {
                response.SetInternalServerErrorStatusCode();
                response.SetErrorMessages(ex.Message);
                return response;
            }
        }

        private string GenerateToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtBearerAuthentication:JwtSecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtBearerAuthentication:JwtTokenExpiresMinutes"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtBearerAuthentication:JwtIssuer"],
                audience: _configuration["JwtBearerAuthentication:JwtIssuer"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtBearerAuthentication:JwtSecretKey"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        #endregion
    }
}
