using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Roulette.Services.GameHistories.Models.Read
{
    public class GameHistoryPreviewDto
    {
        public Guid Id { get; set; }
        public int SpinId { get; set; }
        public decimal BetAmount { get; set; }
        public decimal WonAmount { get; set; }
        public DateTime CreateDate { get; set; }

        public Guid? UserId { get; set; }

        // Map GameHistory entity properties in GameHistory preview model
        public static Expression<Func<Domain.Entities.GameHistory, GameHistoryPreviewDto>> Projection
        => gameHistory => gameHistory == null ? null : new GameHistoryPreviewDto
        {
            Id = gameHistory.Id,
            SpinId = gameHistory.SpinId,
            BetAmount = gameHistory.BetAmount,
            WonAmount = gameHistory.WonAmount,
            CreateDate = gameHistory.CreateDate,
            UserId = gameHistory.UserId
        };
    }
}
