using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TesteBackendEnContact.Controllers.Models;
using TesteBackendEnContact.Core.CrossCutting.Notifications;
using TesteBackendEnContact.Core.Domain.ContactBook.Company;
using TesteBackendEnContact.Core.Interface.ContactBook.Company;

namespace TesteBackendEnContact.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompanyController : ControllerBase
    {
        private INotifier _notifier;
        private readonly ICompanyService _companyService;

        public CompanyController(
            INotifier notifier,
            ICompanyService companyService)
        {
            _notifier = notifier;
            _companyService = companyService;
        }

        [HttpPost]
        public async Task<ActionResult<ICompany>> Post(SaveCompanyRequest company)
        {
            if (!ModelState.IsValid) return BadRequest("invalid parameters");

            var result = await _companyService.Create(company.ToCompany());

            if(_notifier.HasNotification()) return BadRequest(_notifier.GetNotifications());

            return Ok(result);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            await _companyService.Delete(id);

            if (_notifier.HasNotification()) return BadRequest(_notifier.GetNotifications());

            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ICompany>>> Get([FromQuery] CompanyFilter filter)
        {
            var result = await _companyService.GetAllWithFilters(filter);

            if (_notifier.HasNotification()) return NotFound(_notifier.GetNotifications());

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ICompany>> Get(int id)
        {
            var result = await _companyService.GetById(id);

            if (_notifier.HasNotification()) return NotFound(_notifier.GetNotifications());

            return Ok(result);
        }

    }
}
