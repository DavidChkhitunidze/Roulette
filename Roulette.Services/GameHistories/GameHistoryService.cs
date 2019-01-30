using Microsoft.EntityFrameworkCore;
using Roulette.Domain.Entities;
using Roulette.Persistance;
using Roulette.Services.GameHistories.Abstracts;
using Roulette.Services.GameHistories.Models.Read;
using Roulette.Services.GameHistories.Models.Write;
using Roulette.Services.Responses;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Roulette.Services.GameHistories
{
    public class GameHistoryService : IGameHisotyService
    {
        #region PrivateFields

        private readonly RouletteDbContext _db;

        #endregion

        #region Constuctor

        public GameHistoryService(RouletteDbContext db)
        {
            _db = db;
        }

        #endregion

        #region GetMethods

        public async Task<Response<GameHistoryPreviewDto>> GetGameHistoryAsync(Guid? id)
        {
            var response = new Response<GameHistoryPreviewDto>();

            if (id == null)
            {
                response.SetBadRequestStatusCode();
                return response;
            }

            try
            {
                var gameHistoryDto = GameHistoryPreviewDto.Projection.Compile()
                    .Invoke(await _db.Set<GameHistory>().SingleOrDefaultAsync(i => i.Id == id));

                if (gameHistoryDto == null)
                {
                    response.SetNotFountStatusCode();
                    response.SetErrorMessages("Game history not found.");
                    return response;
                }

                response.SetSuccess();
                response.SetOkStatusCode();
                response.SetModel(gameHistoryDto);

                return response;
            }
            catch (Exception ex)
            {
                response.SetInternalServerErrorStatusCode();
                response.SetErrorMessages(ex.Message);
                return response;
            }
        }

        public Response<IQueryable<GameHistoryPreviewDto>> GetGameHistories()
        {
            var response = new Response<IQueryable<GameHistoryPreviewDto>>();

            try
            {
                var gameHistoriesDto = _db.Set<GameHistory>()
                    .OrderByDescending(gh => gh.SpinId)
                    .Select(GameHistoryPreviewDto.Projection);

                if (gameHistoriesDto == null || !gameHistoriesDto.Any())
                {
                    response.SetNotFountStatusCode();
                    response.SetErrorMessages("Game histories not found.");
                    return response;
                }

                response.SetSuccess();
                response.SetOkStatusCode();
                response.SetModel(gameHistoriesDto);

                return response;
            }
            catch (Exception ex)
            {
                response.SetInternalServerErrorStatusCode();
                response.SetErrorMessages(ex.Message);
                return response;
            }
        }

        public Response<IQueryable<GameHistoryPreviewDto>> GetGameHistoriesByUserId(Guid? userId)
        {
            var response = new Response<IQueryable<GameHistoryPreviewDto>>();

            if (userId == null)
            {
                response.SetBadRequestStatusCode();
                return response;
            }

            try
            {
                var gameHistoriesDto = _db.Set<GameHistory>()
                    .Where(gh => gh.UserId == userId)
                    .OrderByDescending(gh => gh.SpinId)
                    .Select(GameHistoryPreviewDto.Projection);

                if (gameHistoriesDto == null || !gameHistoriesDto.Any())
                {
                    response.SetNotFountStatusCode();
                    response.SetErrorMessages("Game histories not found.");
                    return response;
                }

                response.SetSuccess();
                response.SetOkStatusCode();
                response.SetModel(gameHistoriesDto);

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

        public async Task<Response<GameHistoryPreviewDto>> CreateAsync(CreateGameHistoryDto model)
        {
            var response = new Response<GameHistoryPreviewDto>();

            if (model == null)
            {
                response.SetBadRequestStatusCode();
                return response;
            }

            try
            {
                var gameHistoryEntity = CreateGameHistoryDto.CreateEntity.Compile().Invoke(model);
                if (gameHistoryEntity == null)
                {
                    response.SetAcceptedStatusCode();
                    response.SetErrorMessages("Creating game history failed.");
                    return response;
                }

                await _db.Set<GameHistory>().AddAsync(gameHistoryEntity);
                await _db.SaveChangesAsync();

                var gameHistoryDto = GameHistoryPreviewDto.Projection.Compile().Invoke(gameHistoryEntity);

                response.SetSuccess();
                response.SetOkStatusCode();
                response.SetModel(gameHistoryDto);

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
    }
}
