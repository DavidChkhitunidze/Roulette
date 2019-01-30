using Roulette.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Roulette.Services.Users.Models.Write
{
    public class UpdateUserDto
    {
        public Guid Id { get; set; }
        public decimal BetAmount { get; set; }

        public static Func<UpdateUserDto, User, User> UpdateEntity
        => (updateUserDto, userEntity) =>
        {
            if (updateUserDto == null || userEntity == null)
                return null;
        
            userEntity.UserBalance -= updateUserDto.BetAmount;
        
            return userEntity;
        };
    }
}
