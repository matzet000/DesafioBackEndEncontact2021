using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TesteBackendEnContact.Core.Domain.ContactBook.Contact;

namespace TesteBackendEnContact.Core.Interface.ContactBook.Contact
{
    public interface IContactService
    {
        Task<IContact> Create(IContact entity);
        Task<IContact> GetById(int IdEntity);
        Task<IEnumerable<IContact>> GetAllWithFilters(ContactFilter filter);
        Task Delete(int IdEntity);
        Task<ContactImportList> Import(IFormFile file);
        Task<string> Export();
    }
}
