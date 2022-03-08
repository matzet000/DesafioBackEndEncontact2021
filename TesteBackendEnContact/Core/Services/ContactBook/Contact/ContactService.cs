using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TesteBackendEnContact.Core.CrossCutting.Notifications;
using TesteBackendEnContact.Core.Domain.ContactBook.Contact;
using TesteBackendEnContact.Core.Interface.ContactBook.Contact;
using TesteBackendEnContact.Repository.Interface;

namespace TesteBackendEnContact.Core.Services.ContactBook.Contact
{
    public class ContactService : IContactService
    {
        private INotifier _notifier;
        private IContactRepository _contactRepository;
        private IContactBookRepository _contactBookRepository;
        private readonly ICompanyRepository _companyRepository;

        public ContactService(
            INotifier notifier,
            IContactRepository contactRepository,
            IContactBookRepository contactBookRepository,
            ICompanyRepository companyRepository)
        {
            _notifier = notifier;
            _contactRepository = contactRepository;
            _contactBookRepository = contactBookRepository;
            _companyRepository = companyRepository;
        }

        public async Task<IContact> Create(IContact entity)
        {
            if (!Validate(entity)) return null;

            if (entity.CompanyId > 0)
            {
                var company = await _companyRepository.GetAsync(entity.CompanyId);

                if (company == null)
                {
                    _notifier.Handle("Company not found");
                    return null;
                }
            }

            var contactBook = await _contactBookRepository.GetAsync(entity.ContactBookId);

            if (contactBook == null)
            {
                _notifier.Handle("ContactBook not found");
                return null;
            }

            var entitySave = await _contactRepository.SaveAsync(entity);

            return entitySave;
        }

        public async Task Delete(int IdEntity)
        {
            var entity = await _contactBookRepository.GetAsync(IdEntity);

            if (entity == null)
            {
                _notifier.Handle("Contact not found");
                return;
            }

            await _contactRepository.DeleteAsync(IdEntity);
        }

        public async Task<IEnumerable<IContact>> GetAllWithFilters(ContactFilter filter)
        {
            var entity = await _contactRepository.GetAllWithFilters(filter);

            if (entity == null) _notifier.Handle("Not found");

            return entity;
        }

        public async Task<IContact> GetById(int IdEntity)
        {
            var entity = await _contactRepository.GetAsync(IdEntity);

            if (entity == null) _notifier.Handle("Not found");

            return entity;
        }

        private bool Validate(IContact company)
        {
            var validation = new ContactValidation().Validate(company);

            if (validation.IsValid) return true;

            _notifier.Handle("Entity is Invalid");

            return false;
        }

        public async Task<ContactImportList> Import(IFormFile file)
        {
            var list = new ContactImportList();
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                string[] headers = reader.ReadLine().Split(',');
                int line = 2;

                while (!reader.EndOfStream)
                {
                    string[] rows = reader.ReadLine().Split(',');
                    var contact = new Core.Domain.ContactBook.Contact.Contact();

                    int valueTemp;

                    if (int.TryParse(rows[0], out valueTemp))
                    {
                        contact.Id = valueTemp;
                    }

                    contact.Name = rows[1];
                    contact.Phone = rows[2];
                    contact.Email = rows[3];
                    contact.Address = rows[4];

                    if (int.TryParse(rows[5], out valueTemp))
                    {
                        contact.ContactBookId = valueTemp;
                    }
                    else
                    {
                        _notifier.Handle($"Entity is Invalid: line {line}");
                        list.NotProcessed.Add(contact);
                        line++;
                        continue;
                    }

                    if (int.TryParse(rows[6], out valueTemp))
                    {
                        contact.CompanyId = valueTemp;
                    }

                    if (!Validate(contact))
                    {
                        _notifier.Handle($"Entity is Invalid: line {line}");
                        list.NotProcessed.Add(contact);
                        line++;
                        continue;
                    }

                    if (contact.CompanyId >= 0)
                    {
                        var company = await _companyRepository.GetAsync(contact.CompanyId);

                        if (company == null)
                        {
                            _notifier.Handle($"Entity is Invalid: line {line}");
                            list.NotProcessed.Add(contact);
                            line++;
                            continue;
                        }
                    }

                    var contactBook = await _contactBookRepository.GetAsync(contact.ContactBookId);

                    if (contactBook == null)
                    {
                        _notifier.Handle($"Entity is Invalid: line {line}");
                        list.NotProcessed.Add(contact);
                        line++;
                        continue;
                    }

                    if (!Validate(contact))
                    {
                        _notifier.Handle($"Entity is Invalid: line {line}");
                        list.NotProcessed.Add(contact);
                        line++;
                        continue;
                    }

                    var entitySave = await _contactRepository.SaveAsync(contact);

                    if (entitySave == null)
                    {
                        _notifier.Handle($"Entity is Invalid: line {line}");
                        list.NotProcessed.Add(contact);
                        line++;
                        continue;
                    }

                    _notifier.Handle($"Entity is Invalid: line {line}");
                    list.Processed.Add(contact);
                    line++;
                }

                return list;
            }
        }

        public async Task<string> Export()
        {
            var data = await _contactRepository.GetAllAsync();

            var dataString = new StringBuilder();
            dataString.AppendLine("Id,Name,Phone,Email,Address,ContactBookId,CompanyId");

            foreach (var contact in data)
            {
                dataString.AppendLine($"{contact.Id.ToString()},{contact.Name},{contact.Phone},{contact.Email},{contact.Address},{contact.ContactBookId.ToString()},{contact.CompanyId.ToString()},");
            }

            return dataString.ToString();
        }
    }
}
