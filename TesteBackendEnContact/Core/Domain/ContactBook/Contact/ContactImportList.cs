using System.Collections.Generic;
using TesteBackendEnContact.Core.Interface.ContactBook.Contact;

namespace TesteBackendEnContact.Core.Domain.ContactBook.Contact
{
    public class ContactImportList
    {        

        public List<IContact> Processed { get; set; }
        public List<IContact> NotProcessed { get; set; }

        public ContactImportList()
        {
            Processed = new List<IContact>();
            NotProcessed = new List<IContact>();
        }
    }
}
