using Roulette.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Roulette.Services.Jackpots.Models.Write
{
    public class UpdateJackpotDto
    {
        public decimal BetAmount { get; set; }

        public static Func<UpdateJackpotDto, Jackpot, Jackpot> UpdateEntity
        => (updateJackpotDto, jackpotEntity) =>
        {
            if (updateJackpotDto == null || jackpotEntity == null)
                return null;

            jackpotEntity.JackpotAmount += updateJackpotDto.BetAmount * 0.01m;

            return jackpotEntity;
        };
    }
}
