using Roulette.Services.Jackpots.Models.Read;
using Roulette.Services.Jackpots.Models.Write;
using Roulette.Services.Responses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Roulette.Services.Jackpots.Abstracts
{
    public interface IJackpotService
    {
        Task<Response<JackpotPreviewDto>> GetJackpotAsync();
        Task<Response<bool>> JackpotExistsAsync();
        Task<Response<JackpotPreviewDto>> CreateAsync(CreateJackpotDto model);
        Task<Response<JackpotPreviewDto>> UpdateAsync(UpdateJackpotDto model);
    }
}
