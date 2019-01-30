using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Roulette.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        public User()
        {
            GameHistories = new HashSet<GameHistory>();
        }

        public override Guid Id { get; set; } = Guid.NewGuid();

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal UserBalance { get; set; }

        public string RefreshToken { get; set; }

        public ICollection<GameHistory> GameHistories { get; set; }
    }
}
