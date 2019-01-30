using Roulette.Services.Responses;
using Roulette.Services.Users.Models.Read;
using Roulette.Services.Users.Models.Write;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Roulette.Services.Users.Abstracts
{
    public interface IUserService
    {
        Task<Response<UserPreviewDto>> GetUserAsync(Guid? id);
        Task<Response<UserPreviewDto>> LoginAsync(LoginUserDto model);
        Task<Response<UserPreviewDto>> RegisterAsync(RegisterUserDto model);
        Task<Response<UserPreviewDto>> UpdateAsync(UpdateUserDto model);
        Task<Response<UserPreviewDto>> RefreshTokenAsync(string token, string refreshToken);
    }
}
