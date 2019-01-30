using Roulette.Services.Bets.Models.Read;
using Roulette.Services.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Roulette.Services.Bets.Abstracts
{
    public interface IBetService
    {
        Response<BetPreviewDto> Bet(string bet, decimal userBalance);
    }
}
