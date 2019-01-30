using System;
using System.Collections.Generic;
using System.Text;

namespace Roulette.Domain.Entities
{
    public class GameHistory
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int SpinId { get; set; }
        public decimal BetAmount { get; set; }
        public decimal WonAmount { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;

        public Guid? UserId { get; set; }
        public User User { get; set; }
    }
}
