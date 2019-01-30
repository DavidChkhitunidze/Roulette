using Roulette.Services.Bets.Abstracts;
using Roulette.Services.Bets.Models.Read;
using ge.singular.roulette;
using System;
using Roulette.Services.Responses;

namespace Roulette.Services.Bets
{
    public class BetService : IBetService
    {
        #region PostMethods

        public Response<BetPreviewDto> Bet(string bet, decimal userBalance)
        {
            var response = new Response<BetPreviewDto>();

            if (string.IsNullOrEmpty(bet))
            {
                response.SetBadRequestStatusCode();
                return response;
            }

            try
            {
                var betResponse = CheckBets.IsValid(bet);
                if (betResponse == null)
                {
                    response.SetBadRequestStatusCode();
                    return response;
                }

                var betDto = BetPreviewDto.Projection.Compile().Invoke(betResponse.getIsValid(), betResponse.getBetAmount(), 0);
                if (!betDto.IsValid)
                {
                    response.SetConflictStatusCode();
                    response.SetErrorMessages("Bet is not valid. Try again.");
                    return response;
                }

                if (userBalance < 0 || (userBalance - betDto.BetAmount) < 0)
                {
                    response.SetAcceptedStatusCode();
                    response.SetErrorMessages("You don't have enough balance");
                    return response;
                }

                var winningNumber = new Random().Next(0, 36);
                var wonAmount = CheckBets.EstimateWin(bet, winningNumber);

                betDto.WonAmount = wonAmount;

                response.SetSuccess();
                response.SetOkStatusCode();
                response.SetModel(betDto);

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
