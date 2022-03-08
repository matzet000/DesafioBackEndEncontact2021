using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TesteBackendEnContact.Core.Domain.ContactBook.Contact;
using TesteBackendEnContact.Core.Interface.ContactBook.Contact;
using TesteBackendEnContact.Database;
using TesteBackendEnContact.Repository.Interface;

namespace TesteBackendEnContact.Repository
{
    public class ContactRepository : IContactRepository
    {
        private readonly DatabaseConfig databaseConfig;

        public ContactRepository(DatabaseConfig databaseConfig)
        {
            this.databaseConfig = databaseConfig;
        }

        public async Task<IContact> SaveAsync(IContact contact)
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);
            var dao = new ContactDao(contact);

            if (contact.Id == 0)
                dao.Id = await connection.InsertAsync(dao);
            else
                await connection.UpdateAsync(dao);

            return dao.Export();
        }

        public async Task<IEnumerable<IContact>> GetAllWithFilters(ContactFilter filter)
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);

            var query = "";
            IEnumerable<ContactDao> result;

            if (filter != null && (filter.Page != 0 || !string.IsNullOrEmpty(filter.Name) || filter.Id != 0))
            {
                var parameters = new
                {
                    Id = filter.Id,
                    Name = filter.Name,
                    Page = filter.Page,
                };

                if (!string.IsNullOrEmpty(filter.Name))
                {
                    query = @"SELECT * FROM Contact
                        Where Name = @Name ";
                }
                else
                {
                    query = @"SELECT * FROM Contact ";
                }

                if (filter.Page > 0)
                {
                    query += $"LIMIT {filter.Page * 10}, 10;";
                }
                else
                {
                    query += $"LIMIT  10;";
                }

                if (filter.Id != 0)
                {
                    query = @"SELECT * FROM Contact
                        Where Id = @Id";
                }

                result = await connection.QueryAsync<ContactDao>(query, parameters);
                return result?.Select(item => item.Export());
            }

            query = "SELECT * FROM Contact LIMIT 10";
            result = await connection.QueryAsync<ContactDao>(query);

            return result?.Select(item => item.Export());
        }

        public async Task<IContact> GetAsync(int id)
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);

            var query = "SELECT * FROM Contact Where Id = @id";
            var result = await connection.QueryAsync<ContactDao>(query, new { id });

            return result?.FirstOrDefault().Export();
        }

        public async Task<IEnumerable<IContact>> GetAllAsync()
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);

            var query = "SELECT * FROM Contact";
            var result = await connection.QueryAsync<ContactDao>(query);

            return result?.Select(item => item.Export());
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);

            var sql = "DELETE FROM Contact WHERE Id = @id";

            await connection.ExecuteAsync(sql, new { id });
        }

    }

    [Table("Contact")]
    public class ContactDao : IContact
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public int ContactBookId { get; set; }

        public int CompanyId { get; set; }

        public ContactDao()
        {
        }

        public ContactDao(IContact contact)
        {
            Id = contact.Id;
            Name = contact.Name;
            Phone = contact.Phone;
            Email = contact.Email;
            Address = contact.Address;
            ContactBookId = contact.ContactBookId;
            CompanyId = contact.CompanyId;
        }

        public IContact Export() => new Contact(Id, Name, Phone, Email, Address, ContactBookId, CompanyId);
    }
}
