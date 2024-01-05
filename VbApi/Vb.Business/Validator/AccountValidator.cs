using FluentValidation;
using Vb.Base.Common;
using Vb.Schema;

namespace Vb.Business.Validator;

public class CreateAccountValidator: AbstractValidator<AccountRequest>
{
    public CreateAccountValidator()
    {
        RuleFor(x => x.IBAN).NotEmpty().MaximumLength(34);
        
        RuleFor(x => x.Balance)
            .NotEmpty().Must(CommonValidators.HaveValidScale).WithMessage("Amount must have a maximum of 4 decimal places")
            .Must(CommonValidators.HaveValidPrecision).WithMessage("Amount must have a maximum of 18 digits in total");

        RuleFor(x => x.CurrencyType).NotEmpty().MaximumLength(3);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);

    }
}