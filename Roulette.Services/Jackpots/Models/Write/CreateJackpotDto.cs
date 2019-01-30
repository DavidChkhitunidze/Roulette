using Roulette.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Roulette.Services.Jackpots.Models.Write
{
    public class CreateJackpotDto
    {
        public decimal JackpotAmout { get; set; }

        public static Expression<Func<CreateJackpotDto, Jackpot>> CreateEntity 
        => createJackpotDto => createJackpotDto == null ? null : new Jackpot
        {
            JackpotAmount = createJackpotDto.JackpotAmout
        };
    }
}
