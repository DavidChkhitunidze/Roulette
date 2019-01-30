using Roulette.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Roulette.Services.Users.Models.Read
{
    public class UserPreviewDto
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal UserBalance { get; set; }
        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }

        // Map User entity properties in User preview model
        public static Expression<Func<User, UserPreviewDto>> Projection 
        => user => user == null ? null : new UserPreviewDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserBalance = user.UserBalance,
            RefreshToken = user.RefreshToken
        };
    }
}
