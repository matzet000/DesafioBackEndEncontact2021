using FluentValidation;
using TesteBackendEnContact.Core.Interface.ContactBook.Company;

namespace TesteBackendEnContact.Core.Domain.ContactBook.Company
{
    public class CompanyValidation : AbstractValidator<ICompany>
    {
        public CompanyValidation()
        {
            RuleFor(c => c.Name)
               .NotEmpty().WithMessage("The {PropertyName} needs to be provided")
               .MaximumLength(50).WithMessage("The {PropertyName} need to have max {MaxLength} characters");
        }
    }
}
