using System.Collections.Generic;
using System.Threading.Tasks;
using TesteBackendEnContact.Core.Domain.ContactBook;

namespace TesteBackendEnContact.Core.Interface.ContactBook
{
    public interface IContactBookService
    {
        Task<IContactBook> Create(IContactBook entity);
        Task<IContactBook> GetById(int IdEntity);
        Task<IEnumerable<IContactBook>> GetAllWithFilters(ContactBookFilter filter);
        Task Delete(int IdEntity);
    }
}
