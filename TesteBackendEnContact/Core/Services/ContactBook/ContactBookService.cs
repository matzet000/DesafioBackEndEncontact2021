using System.Collections.Generic;
using System.Threading.Tasks;
using TesteBackendEnContact.Core.CrossCutting.Notifications;
using TesteBackendEnContact.Core.Domain.ContactBook;
using TesteBackendEnContact.Core.Interface.ContactBook;
using TesteBackendEnContact.Repository.Interface;

namespace TesteBackendEnContact.Core.Services.ContactBook
{
    public class ContactBookService : IContactBookService
    {
        private INotifier _notifier;
        private readonly IContactBookRepository _contactBookRepository;

        public ContactBookService(INotifier notifier, IContactBookRepository contactBookRepository)
        {
            _notifier = notifier;
            _contactBookRepository = contactBookRepository;
        }

        public async Task<IContactBook> Create(IContactBook entity)
        {
            if (!Validate(entity)) return null;

            var entitySave = await _contactBookRepository.SaveAsync(entity);

            return entitySave;
        }

        public async Task<IContactBook> GetById(int IdEntity)
        {
            var entity = await _contactBookRepository.GetAsync(IdEntity);

            if (entity == null) _notifier.Handle("Not found");

            return entity;
        }

        public async Task<IEnumerable<IContactBook>> GetAllWithFilters(ContactBookFilter filter)
        {
            var entity = await _contactBookRepository.GetAllWithFilters(filter);

            if (entity == null) _notifier.Handle("Not found");

            return entity;
        }

        public async Task Delete(int IdEntity)
        {
            await _contactBookRepository.DeleteAsync(IdEntity);
        }

        private bool Validate(IContactBook company)
        {
            var validation = new ContactBookValidation().Validate(company);

            if (validation.IsValid) return true;

            _notifier.Handle("Entity is Invalid");

            return false;
        }
    }
}
