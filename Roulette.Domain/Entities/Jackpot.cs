using System;

namespace Roulette.Domain.Entities
{
    public class Jackpot
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public decimal JackpotAmount { get; set; }

        public byte[] RowVersion { get; set; }
    }
}
