using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Roulette.Services.Bets.Abstracts;
using Roulette.Services.Bets.Models.Read;
using Roulette.Services.Bets.Models.Write;
using Roulette.Services.GameHistories.Abstracts;
using Roulette.Services.GameHistories.Models.Read;
using Roulette.Services.GameHistories.Models.Write;
using Roulette.Services.Jackpots.Abstracts;
using Roulette.Services.Jackpots.Models.Read;
using Roulette.Services.Jackpots.Models.Write;
using Roulette.Services.Responses;
using Roulette.Services.Users.Abstracts;
using Roulette.Services.Users.Models.Read;
using Roulette.Services.Users.Models.Write;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Roulette.Api.Controllers
{
    [ProducesResponseType(500, Type = typeof(Response))]
    [ProducesResponseType(401)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RouletteController : BaseController
    {
        #region PrivateFields

        private readonly IGameHisotyService _gameHisoty;
        private readonly IJackpotService _jackpot;
        private readonly IUserService _user;
        private readonly IBetService _bet;

        #endregion

        #region Constructor

        public RouletteController(
            IGameHisotyService gameHisoty,
            IJackpotService jackpot,
            IUserService user,
            IBetService bet)
        {
            _gameHisoty = gameHisoty;
            _jackpot = jackpot;
            _user = user;
            _bet = bet;
        }

        #endregion

        #region HttpGetMethods

        #region GameHistory

        [ProducesResponseType(400, Type = typeof(Response))]
        [ProducesResponseType(404, Type = typeof(Response))]
        [ProducesResponseType(200, Type = typeof(Response<GameHistoryPreviewDto>))]
        [HttpGet("gamehistory", Name = "GetGameHistory")]
        public async Task<ActionResult<Response<GameHistoryPreviewDto>>> GetGameHistory(Guid? id)
        {
            var gameHidtoryResponse = await _gameHisoty.GetGameHistoryAsync(id);
            if (!gameHidtoryResponse.Success)
                return StatusCode(gameHidtoryResponse.StatusCode, gameHidtoryResponse);

            return StatusCode(gameHidtoryResponse.StatusCode, gameHidtoryResponse);
        }

        [ProducesResponseType(404, Type = typeof(Response))]
        [ProducesResponseType(200, Type = typeof(Response<IQueryable<GameHistoryPreviewDto>>))]
        [HttpGet("gamehistories", Name = "GetGameHistories")]
        public ActionResult<Response<IQueryable<GameHistoryPreviewDto>>> GetGameHistories()
        {
            var gameHistoriesResponse = _gameHisoty.GetGameHistories();
            if (!gameHistoriesResponse.Success)
                return StatusCode(gameHistoriesResponse.StatusCode, gameHistoriesResponse);

            return StatusCode(gameHistoriesResponse.StatusCode, gameHistoriesResponse);
        }

        [ProducesResponseType(404, Type = typeof(Response))]
        [ProducesResponseType(400, Type = typeof(Response))]
        [ProducesResponseType(200, Type = typeof(Response<IQueryable<GameHistoryPreviewDto>>))]
        [HttpGet("gamehistories/{userId}", Name = "GetGameHistoriesbyUserId")]
        public ActionResult<Response<IQueryable<GameHistoryPreviewDto>>> GetGameHistoriesByUserId(Guid? userId)
        {
            var gameHistoriesResponse = _gameHisoty.GetGameHistoriesByUserId(userId);
            if (!gameHistoriesResponse.Success)
                return StatusCode(gameHistoriesResponse.StatusCode, gameHistoriesResponse);

            return StatusCode(gameHistoriesResponse.StatusCode, gameHistoriesResponse);
        }

        #endregion

        #region Jackpot

        [ProducesResponseType(404, Type = typeof(Response))]
        [ProducesResponseType(200, Type = typeof(Response<JackpotPreviewDto>))]
        [HttpGet("jackpot", Name = "GetJackpot")]
        public async Task<ActionResult<Response<JackpotPreviewDto>>> GetJackpot()
        {
            var jackpotResponse = await _jackpot.GetJackpotAsync();
            if (!jackpotResponse.Success)
                return StatusCode(jackpotResponse.StatusCode, jackpotResponse);

            return StatusCode(jackpotResponse.StatusCode, jackpotResponse);
        }

        #endregion

        #region User

        [ProducesResponseType(404, Type = typeof(Response))]
        [ProducesResponseType(400, Type = typeof(Response))]
        [ProducesResponseType(200, Type = typeof(Response<UserPreviewDto>))]
        [HttpGet("user", Name = "GetUser")]
        public async Task<ActionResult<Response<UserPreviewDto>>> GetUser(Guid? id)
        {
            var userResponse = await _user.GetUserAsync(id);
            if (!userResponse.Success)
                return StatusCode(userResponse.StatusCode, userResponse);

            return StatusCode(userResponse.StatusCode, userResponse);
        }

        #endregion

        #endregion

        #region HttpPostMethods

        #region Bet

        [ProducesResponseType(409, Type = typeof(Response))]
        [ProducesResponseType(404, Type = typeof(Response))]
        [ProducesResponseType(400, Type = typeof(Response))]
        [ProducesResponseType(202, Type = typeof(Response))]
        [ProducesResponseType(200, Type = typeof(Response<BetPreviewDto>))]
        [HttpPost("bet")]
        public async Task<ActionResult<Response<BetPreviewDto>>> Bet([FromForm] CreateBetDto model)
        {
            var userResponse = await _user.GetUserAsync(model.UserId);
            if (!userResponse.Success)
                return StatusCode(userResponse.StatusCode, userResponse);

            var betResponse = _bet.Bet(model.Bet, userResponse.Model.UserBalance);
            if (!betResponse.Success)
                return StatusCode(betResponse.StatusCode, betResponse);

            var updatedUserResponse = await _user.UpdateAsync(new UpdateUserDto
            {
                BetAmount = betResponse.Model.BetAmount,
                Id = userResponse.Model.Id
            });

            if (!updatedUserResponse.Success)
                return StatusCode(updatedUserResponse.StatusCode, updatedUserResponse);

            var createdGameHistoryResponse = await _gameHisoty.CreateAsync(new CreateGameHistoryDto
            {
                UserId = userResponse.Model.Id,
                BetAmount = betResponse.Model.BetAmount,
                WonAmount = betResponse.Model.WonAmount
            });

            if (!createdGameHistoryResponse.Success)
                return StatusCode(createdGameHistoryResponse.StatusCode, createdGameHistoryResponse);

            var updatedJackpotResponse = await _jackpot.UpdateAsync(new UpdateJackpotDto
            {
                BetAmount = betResponse.Model.BetAmount
            });

            if (!updatedJackpotResponse.Success)
                return StatusCode(updatedJackpotResponse.StatusCode, updatedJackpotResponse);

            return StatusCode(betResponse.StatusCode, betResponse);
        }

        #endregion

        #region Jackpot

        [ProducesResponseType(409, Type = typeof(Response))]
        [ProducesResponseType(400, Type = typeof(Response))]
        [ProducesResponseType(202, Type = typeof(Response))]
        [ProducesResponseType(201, Type = typeof(Response<JackpotPreviewDto>))]
        [HttpPost("jackpot")]
        public async Task<ActionResult<Response<JackpotPreviewDto>>> CreateJackpot([FromForm] CreateJackpotDto model)
        {
            var jackpotExistsResponse = await _jackpot.JackpotExistsAsync();

            if (!jackpotExistsResponse.Success)
                return StatusCode(jackpotExistsResponse.StatusCode, jackpotExistsResponse);

            var jackpotResponse = await _jackpot.CreateAsync(model);
            if (!jackpotResponse.Success)
                return StatusCode(jackpotResponse.StatusCode, jackpotResponse);

            return CreatedAtRoute("GetJackpot", jackpotResponse);
        }

        #endregion

        #endregion
    }
}
