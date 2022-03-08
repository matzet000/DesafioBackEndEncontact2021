using System.Collections.Generic;
using System.Threading.Tasks;
using TesteBackendEnContact.Core.Domain.ContactBook.Company;

namespace TesteBackendEnContact.Core.Interface.ContactBook.Company
{
    public interface ICompanyService
    {
        Task<ICompany> Create(ICompany entity);
        Task<ICompany> GetById(int IdEntity);
        Task<IEnumerable<ICompany>> GetAllWithFilters(CompanyFilter filter);
        Task Delete(int IdEntity);
    }
}
