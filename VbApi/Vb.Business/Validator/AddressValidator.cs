using FluentValidation;
using Vb.Schema;

namespace Vb.Business.Validator;

public class CreateAddressValidator : AbstractValidator<AddressRequest>
{
    public CreateAddressValidator()
    {
        RuleFor(x => x.PostalCode).NotEmpty().MaximumLength(10).MinimumLength(6).WithName("Zip code or postal code");
        RuleFor(x => x.Address1).NotEmpty().MaximumLength(150).WithName("Customer address line 1");
        RuleFor(x => x.Address2).NotEmpty().MaximumLength(150).WithName("Customer address line 2");
        RuleFor(x => x.Country).NotEmpty().MaximumLength(100);
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
        RuleFor(x => x.County).NotEmpty().MaximumLength(100);
        RuleFor(x => x.IsDefault).NotEmpty();
    }
}