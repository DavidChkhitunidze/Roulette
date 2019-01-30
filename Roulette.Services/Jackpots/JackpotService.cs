using Microsoft.EntityFrameworkCore;
using Roulette.Domain.Entities;
using Roulette.Persistance;
using Roulette.Services.Jackpots.Abstracts;
using Roulette.Services.Jackpots.Models.Read;
using Roulette.Services.Jackpots.Models.Write;
using Roulette.Services.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roulette.Services.Jackpots
{
    public class JackpotService : IJackpotService
    {
        #region PrivateFields

        private readonly RouletteDbContext _db;

        #endregion

        #region Constuctor

        public JackpotService(RouletteDbContext db)
        {
            _db = db;
        }

        #endregion

        #region GetMethods

        public async Task<Response<JackpotPreviewDto>> GetJackpotAsync()
        {
            var response = new Response<JackpotPreviewDto>();

            try
            {
                var jackpotDto = JackpotPreviewDto.Projection.Compile().Invoke(await _db.Set<Jackpot>().SingleOrDefaultAsync());
                if (jackpotDto == null)
                {
                    response.SetNotFountStatusCode();
                    response.SetErrorMessages("Jackpot not found.");
                    return response;
                }

                response.SetSuccess();
                response.SetOkStatusCode();
                response.SetModel(jackpotDto);

                return response;
            }
            catch (Exception ex)
            {
                response.SetInternalServerErrorStatusCode();
                response.SetErrorMessages(ex.Message);
                return response;
            }
        }

        public async Task<Response<bool>> JackpotExistsAsync()
        {
            var response = new Response<bool>();

            try
            {
                var jackpotExists = await _db.Set<Jackpot>().AnyAsync();
                if (jackpotExists)
                {
                    response.SetConflictStatusCode();
                    response.SetErrorMessages("Jackpot already exists.");
                    return response;
                }

                response.SetOkStatusCode();
                response.SetSuccess();
                response.SetModel(jackpotExists);

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

        public async Task<Response<JackpotPreviewDto>> CreateAsync(CreateJackpotDto model)
        {
            var response = new Response<JackpotPreviewDto>();

            if (model == null)
            {
                response.SetBadRequestStatusCode();
                return response;
            }

            try
            {
                var jackpotEntity = CreateJackpotDto.CreateEntity.Compile().Invoke(model);
                if (jackpotEntity == null)
                {
                    response.SetAcceptedStatusCode();
                    response.SetErrorMessages("Creating jackpot failed.");
                    return response;
                }    

                await _db.Set<Jackpot>().AddAsync(jackpotEntity);
                await _db.SaveChangesAsync();

                var jackpotDto = JackpotPreviewDto.Projection.Compile().Invoke(jackpotEntity);

                response.SetOkStatusCode();
                response.SetSuccess();
                response.SetModel(jackpotDto);

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

        #region PutMethods

        public async Task<Response<JackpotPreviewDto>> UpdateAsync(UpdateJackpotDto model)
        {
            var response = new Response<JackpotPreviewDto>();

            if (model == null)
            {
                response.SetBadRequestStatusCode();
                return response;
            }

            try
            {
                var jackpotEntity = await _db.Set<Jackpot>().SingleOrDefaultAsync();
                if (jackpotEntity == null)
                {
                    response.SetNotFountStatusCode();
                    response.SetErrorMessages("Jackpot not found.");
                    return response;
                }

                bool saveFailed;
                do
                {
                    saveFailed = false;
                    try
                    {
                        var updatedJackpotEntity = UpdateJackpotDto.UpdateEntity(model, jackpotEntity);
                        if (updatedJackpotEntity == null)
                        {
                            response.SetAcceptedStatusCode();
                            response.SetErrorMessages("Updating jackpot failed.");
                            return response;
                        }

                        _db.Set<Jackpot>().Update(updatedJackpotEntity);
                        await _db.SaveChangesAsync();

                        var updatedJackpotDto = JackpotPreviewDto.Projection.Compile().Invoke(updatedJackpotEntity);

                        response.SetSuccess();
                        response.SetOkStatusCode();
                        response.SetModel(updatedJackpotDto);

                        return response;
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        saveFailed = true;
                        ex.Entries.Single().Reload();
                    }

                } while (saveFailed);

                response.SetAcceptedStatusCode();
                response.SetErrorMessages("Updating jackpot failed.");
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
