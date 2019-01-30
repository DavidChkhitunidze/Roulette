using Roulette.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Roulette.Services.Jackpots.Models.Read
{
    public class JackpotPreviewDto
    {
        public decimal JackpotAmount { get; set; }

        // Map Jackpot entity properties in Jackpot preview model
        public static Expression<Func<Jackpot, JackpotPreviewDto>> Projection 
        => jackpot => jackpot == null ? null : new JackpotPreviewDto
        {
            JackpotAmount = jackpot.JackpotAmount
        };
    }
}
