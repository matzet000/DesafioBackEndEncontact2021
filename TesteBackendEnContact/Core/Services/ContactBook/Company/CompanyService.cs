using System.Collections.Generic;
using System.Threading.Tasks;
using TesteBackendEnContact.Core.CrossCutting.Notifications;
using TesteBackendEnContact.Core.Domain.ContactBook.Company;
using TesteBackendEnContact.Core.Interface.ContactBook.Company;
using TesteBackendEnContact.Repository.Interface;

namespace TesteBackendEnContact.Core.Services.ContactBook.Company
{
    public class CompanyService : ICompanyService
    {
        private INotifier _notifier;
        private readonly ICompanyRepository _companyRepository;
        private readonly IContactBookRepository _contactBookRepository;

        public CompanyService(
            INotifier notifier, 
            ICompanyRepository companyRepository,
            IContactBookRepository contactBookRepository)
        {
            _notifier = notifier;
            _companyRepository = companyRepository;
            _contactBookRepository = contactBookRepository;
        }

        public async Task<ICompany> Create(ICompany entity)
        {
            if (!Validate(entity)) return null;

            if(entity.ContactBookId > 0)
            {
                var contactBook = await _contactBookRepository.GetAsync(entity.ContactBookId);

                if(contactBook == null)
                {
                    _notifier.Handle("ContactBook invalid");
                    return null;
                }
            }

            var entitySave = await _companyRepository.SaveAsync(entity);

            return entitySave;
        }

        public async Task Delete(int IdEntity)
        {
            var entity = await _companyRepository.GetAsync(IdEntity);

            if(entity == null)
            {
                _notifier.Handle("Company not found");
                return;
            }

            await _companyRepository.DeleteAsync(IdEntity);
        }

        public async Task<IEnumerable<ICompany>> GetAllWithFilters(CompanyFilter filter)
        {
            var entity = await _companyRepository.GetAllWithFiltersAsync(filter);

            if (entity == null) _notifier.Handle("Not found");

            return entity;
        }

        public async Task<ICompany> GetById(int IdEntity)
        {
            var entity = await _companyRepository.GetAsync(IdEntity);

            if (entity == null) _notifier.Handle("Not found");

            return entity;
        }

        private bool Validate(ICompany company)
        {
            var validation = new CompanyValidation().Validate(company);

            if (validation.IsValid) return true;

            _notifier.Handle("Entity is Invalid");

            return false;
        }
    }
}
