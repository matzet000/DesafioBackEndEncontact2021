using FluentValidation;
using TesteBackendEnContact.Core.Interface.ContactBook;

namespace TesteBackendEnContact.Core.Domain.ContactBook
{
    public class ContactBookValidation : AbstractValidator<IContactBook>
    {
        public ContactBookValidation()
        {
            RuleFor(c => c.Name)
              .NotEmpty().WithMessage("The {PropertyName} needs to be provided")
              .MaximumLength(50).WithMessage("The {PropertyName} need to have max {MaxLength} characters");
        }
    }
}
