using System.Collections.Generic;
using System.Threading.Tasks;
using TesteBackendEnContact.Core.Domain.ContactBook;
using TesteBackendEnContact.Core.Interface.ContactBook;

namespace TesteBackendEnContact.Repository.Interface
{
    public interface IContactBookRepository
    {
        Task<IContactBook> SaveAsync(IContactBook contactBook);
        Task DeleteAsync(int id);
        Task<IContactBook> GetAsync(int id);
        Task<IEnumerable<IContactBook>> GetAllWithFilters(ContactBookFilter filter);
        Task<IEnumerable<IContactBook>> GetContactBookByCompanyAsync(int id);
    }
}
