using Roulette.Services.GameHistories.Models.Read;
using Roulette.Services.GameHistories.Models.Write;
using Roulette.Services.Responses;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Roulette.Services.GameHistories.Abstracts
{
    public interface IGameHisotyService
    {
        Task<Response<GameHistoryPreviewDto>> GetGameHistoryAsync(Guid? id);
        Response<IQueryable<GameHistoryPreviewDto>> GetGameHistories();
        Response<IQueryable<GameHistoryPreviewDto>> GetGameHistoriesByUserId(Guid? userId);
        Task<Response<GameHistoryPreviewDto>> CreateAsync(CreateGameHistoryDto model);
    }
}
