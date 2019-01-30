using FluentValidation;
using Roulette.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Roulette.Services.Users.Models.Write
{
    public class RegisterUserDto
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public static Expression<Func<RegisterUserDto, User>> CreateEntity 
        => registerDto => registerDto == null ? null : new User
        {
            Email = registerDto.Email,
            UserName = registerDto.Email,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            UserBalance = 100,
            EmailConfirmed = true
        };
    }

    // User registration property validation rules
    public class RegisterUserValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserValidator()
        {
            RuleFor(r => r.Email).EmailAddress().NotEmpty().MinimumLength(3);
            RuleFor(r => r.FirstName).NotEmpty();
            RuleFor(r => r.LastName).NotEmpty();
            RuleFor(r => r.Password).NotEmpty().MinimumLength(4);
            RuleFor(r => r.ConfirmPassword).Equal(r => r.Password).WithMessage("'Password' and 'Confirm Password' must be equal.").NotEmpty().MinimumLength(4);
        }
    }
}
