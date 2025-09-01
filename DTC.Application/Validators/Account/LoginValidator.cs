using DTC.Application.DTO.Account;
using FluentValidation;

namespace DTC.Application.Validators.Account
{
    public class LoginValidator : AbstractValidator<LoginDTO>
    {
        public LoginValidator() 
        {
            RuleFor(u => u.Username)
                .NotEmpty()
                .WithMessage("Имя пользователя обязательно");
            RuleFor(p => p.Password)
            .NotEmpty().WithMessage("Пароль обязателен.");
        }
    }
}
