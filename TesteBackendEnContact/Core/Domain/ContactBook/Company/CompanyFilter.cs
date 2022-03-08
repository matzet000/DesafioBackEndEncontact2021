namespace TesteBackendEnContact.Core.Domain.ContactBook.Company
{
    public class CompanyFilter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ContactBookId { get; set; }
        public int Page { get; set; }
        public string Word { get; set; }
    }
}
