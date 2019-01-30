using Microsoft.AspNetCore.Identity;
using System;

namespace Roulette.Domain.Entities
{
    public class UserRole : IdentityRole<Guid>
    {
        public override Guid Id { get; set; } = Guid.NewGuid();
    }
}
