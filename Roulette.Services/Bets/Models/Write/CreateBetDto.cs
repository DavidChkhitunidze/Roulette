using System;
using System.Collections.Generic;
using System.Text;

namespace Roulette.Services.Bets.Models.Write
{
    public class CreateBetDto
    {
        public Guid UserId { get; set; }
        public string Bet { get; set; }
    }
}
