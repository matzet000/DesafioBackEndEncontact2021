namespace TesteBackendEnContact.Core.Domain.ContactBook.Contact
{
    public class ContactFilter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public int ContactBookId { get; set; }
        public int CompanyId { get; set; }
        public int Page { get; set; }

        public string Word { get; set; }
    }
}
