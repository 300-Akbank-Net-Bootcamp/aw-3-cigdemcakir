using FluentValidation;
using Vb.Base.Common;
using Vb.Schema;

namespace Vb.Business.Validator;

public class CreateAccountTransactionValidator: AbstractValidator<AccountTransactionRequest>
{
    public CreateAccountTransactionValidator()
    {
        RuleFor(x => x.ReferenceNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.TransactionDate).NotEmpty();
        
        RuleFor(x => x.Amount)
            .NotEmpty().Must(CommonValidators.HaveValidScale).WithMessage("Amount must have a maximum of 4 decimal places")
            .Must(CommonValidators.HaveValidPrecision).WithMessage("Amount must have a maximum of 18 digits in total");

        RuleFor(x => x.Description).NotEmpty().MaximumLength(300);
        RuleFor(x => x.TransferType).NotEmpty().MaximumLength(10);

     }
}