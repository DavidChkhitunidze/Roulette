using FluentValidation;
using Roulette.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Roulette.Services.GameHistories.Models.Write
{
    public class CreateGameHistoryDto
    {
        public decimal BetAmount { get; set; }
        public decimal WonAmount { get; set; }

        public Guid? UserId { get; set; }

        public static Expression<Func<CreateGameHistoryDto, GameHistory>> CreateEntity 
        => gameHistoryDto => gameHistoryDto == null ? null : new GameHistory
        {
            BetAmount = gameHistoryDto.BetAmount,
            WonAmount = gameHistoryDto.WonAmount,
            UserId = gameHistoryDto.UserId
        };
    }
}
