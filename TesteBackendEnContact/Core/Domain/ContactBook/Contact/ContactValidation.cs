using FluentValidation;
using TesteBackendEnContact.Core.Interface.ContactBook.Contact;

namespace TesteBackendEnContact.Core.Domain.ContactBook.Contact
{
    public class ContactValidation : AbstractValidator<IContact>
    {
        public ContactValidation()
        {
            RuleFor(c => c.Name)
              .NotEmpty().WithMessage("The {PropertyName} needs to be provided")
              .MaximumLength(50).WithMessage("The {PropertyName} need to have max {MaxLength} characters");

            RuleFor(c => c.Phone)
                .MaximumLength(20).WithMessage("The {PropertyName} need to have max {MaxLength} characters");

            RuleFor(c => c.Email)
                .MaximumLength(50).WithMessage("The {PropertyName} need to have max {MaxLength} characters");

            RuleFor(c => c.Address)
                .MaximumLength(100).WithMessage("The {PropertyName} need to have max {MaxLength} characters");

            RuleFor(c => c.ContactBookId)
                .NotEmpty()
                .WithMessage("The {PropertyName} needs to be provided");
        }
    }
}
