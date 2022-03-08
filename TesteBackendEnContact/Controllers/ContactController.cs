using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TesteBackendEnContact.Controllers.Models;
using TesteBackendEnContact.Core.CrossCutting.Notifications;
using TesteBackendEnContact.Core.Domain.ContactBook.Contact;
using TesteBackendEnContact.Core.Interface.ContactBook.Contact;

namespace TesteBackendEnContact.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContactController : ControllerBase
    {
        private INotifier _notifier;
        private IContactService _contactService;

        public ContactController(
            INotifier notifier,
            IContactService contactService)
        {
            _notifier = notifier;
            _contactService = contactService;
        }

        [HttpPost]
        public async Task<ActionResult<IContact>> Post(SaveContactRequest saveContact)
        {
            if (!ModelState.IsValid) return BadRequest("invalid parameters");

            var result = await _contactService.Create(saveContact.ToContact());

            if (_notifier.HasNotification()) return BadRequest(_notifier.GetNotifications());

            return Ok(result);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            await _contactService.Delete(id);

            if (_notifier.HasNotification()) return BadRequest(_notifier.GetNotifications());

            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<IContact>>> Get([FromQuery] ContactFilter filter)
        {
            var result = await _contactService.GetAllWithFilters(filter);

            if (_notifier.HasNotification()) return NotFound(_notifier.GetNotifications());

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IContact>> Get(int id)
        {
            var result = await _contactService.GetById(id);

            if (_notifier.HasNotification()) return NotFound(_notifier.GetNotifications());

            return Ok(result);
        }

        [HttpPost("import")]
        public async Task<ActionResult<ContactImportList>> Import([FromForm] IFormFile file)
        {
            if (file == null) return BadRequest("invalid parameters");

            var result = await _contactService.Import(file);

            if(_notifier.HasNotification()) return NotFound(result);

            return Ok(result);
        }

        [HttpGet("export")]
        public async Task<ActionResult> Export()
        {   
            var result = await _contactService.Export();

            return File(Encoding.UTF8.GetBytes(result), "text/csv", "contacts.csv");
        }
    }
}
