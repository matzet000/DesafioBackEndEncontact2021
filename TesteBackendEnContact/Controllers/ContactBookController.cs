using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TesteBackendEnContact.Controllers.Models;
using TesteBackendEnContact.Core.CrossCutting.Notifications;
using TesteBackendEnContact.Core.Domain.ContactBook;
using TesteBackendEnContact.Core.Interface.ContactBook;

namespace TesteBackendEnContact.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContactBookController : ControllerBase
    {
        private INotifier _notifier;
        private readonly IContactBookService _contactBookService;

        public ContactBookController(
            IContactBookService contactBookService,
            INotifier notifier)
        {
            _contactBookService = contactBookService;
            _notifier = notifier;
        }

        [HttpPost]
        public async Task<ActionResult<IContactBook>> Post(SaveContactBookRequest saveContactBook)
        {
            if (!ModelState.IsValid) return BadRequest("invalid parameters");

            var result = await _contactBookService.Create(saveContactBook.ToContactBook());

            if (_notifier.HasNotification()) return BadRequest(_notifier.GetNotifications());

            return Ok(result);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            await _contactBookService.Delete(id);

            if (_notifier.HasNotification()) return BadRequest(_notifier.GetNotifications());

            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<IContactBook>>> Get([FromQuery] ContactBookFilter filter)
        {
            var result = await _contactBookService.GetAllWithFilters(filter);

            if (_notifier.HasNotification()) return NotFound(_notifier.GetNotifications());

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IContactBook>> Get(int id)
        {
            var result = await _contactBookService.GetById(id);

            if(_notifier.HasNotification()) return NotFound(_notifier.GetNotifications());

            return Ok(result);
        }

    }
}
