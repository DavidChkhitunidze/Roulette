using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Roulette.Services.Bets.Models.Read
{
    public class BetPreviewDto
    {
        public bool IsValid { get; set; }
        public decimal BetAmount { get; set; }
        public decimal WonAmount { get; set; }

        // Map Bet properties in Bet preview model
        public static Expression<Func<bool, decimal, decimal, BetPreviewDto>> Projection
        => (isValid, betAmount, wonAmount) => new BetPreviewDto
        {
            IsValid = isValid,
            BetAmount = betAmount,
            WonAmount = wonAmount
        };
    }
}
