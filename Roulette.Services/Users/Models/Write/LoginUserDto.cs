using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Roulette.Services.Users.Models.Write
{
    public class LoginUserDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    // User login property validation rules
    public class LoginUserValidator : AbstractValidator<LoginUserDto>
    {
        public LoginUserValidator()
        {
            RuleFor(l => l.UserName).EmailAddress().NotEmpty().MinimumLength(3);
            RuleFor(l => l.Password).NotNull().MinimumLength(4);
        }
    }
}
