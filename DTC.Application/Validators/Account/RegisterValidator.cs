using DTC.Application.DTO.Account;
using FluentValidation;

namespace DTC.Application.Validators.Account
{
    public class RegisterValidator : AbstractValidator<RegisterDTO>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Имя обязательно для заполнения.")
                .MaximumLength(50).WithMessage("Имя не может быть длиннее 50 символов.")
                .Matches("^[a-zA-Zа-яА-Я'-]*$").WithMessage("Имя может содержать только буквы, дефис и апостроф.");

            RuleFor(x => x.SecondName)
                .MaximumLength(50).WithMessage("Отчество не может быть длиннее 50 символов.")
                .Matches("^[a-zA-Zа-яА-Я'-]*$").When(x => !string.IsNullOrEmpty(x.SecondName)).WithMessage("Отчество может содержать только буквы, дефис и апостроф.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Фамилия обязательна для заполнения.")
                .MaximumLength(50).WithMessage("Фамилия не может быть длиннее 50 символов.")
                .Matches("^[a-zA-Zа-яА-Я'-]*$").WithMessage("Фамилия может содержать только буквы, дефис и апостроф.");

            RuleFor(x => x.Gender)
                .NotEmpty().WithMessage("Необходимо указать пол.")
                .Must(g => new[] { "Мужской", "Женский" }.Contains(g))
                .WithMessage("Пол может быть только 'Мужской' или 'Женский'.");

            RuleFor(x => x.Birthday)
                    .NotEmpty().WithMessage("Дата рождения обязательна.")
                    .Must(BeAValidAge).WithMessage("Пользователь должен быть старше 14 лет и дата рождения не может быть в будущем.");

            RuleFor(x => x.Username)
                    .NotEmpty().WithMessage("Имя пользователя обязательно.")
                    .MinimumLength(3).WithMessage("Имя пользователя должно быть не короче 3 символов.")
                    .MaximumLength(30).WithMessage("Имя пользователя должно быть не длиннее 30 символов.")
                    .Matches("^[a-zA-Z0-9_]*$").WithMessage("Имя пользователя может содержать только латинские буквы, цифры и знак подчеркивания.");

            RuleFor(x => x.Password)
                    .NotEmpty().WithMessage("Пароль обязателен.")
                    .MinimumLength(8).WithMessage("Пароль должен быть не короче 8 символов.")
                    .Matches("[A-Z]").WithMessage("Пароль должен содержать хотя бы одну заглавную букву.")
                    .Matches("[a-z]").WithMessage("Пароль должен содержать хотя бы одну строчную букву.")
                    .Matches("[0-9]").WithMessage("Пароль должен содержать хотя бы одну цифру.")
                    .Matches("[^a-zA-Z0-9]").WithMessage("Пароль должен содержать хотя бы один специальный символ.");
        }

        private bool BeAValidAge(DateTime birthday)
        {
            if (birthday > DateTime.Today)
                return false;

            var age = DateTime.Today.Year - birthday.Year;
            if (birthday.Date > DateTime.Today.AddYears(-age))
                age--;
            return age >= 14;
        }
    }
}
