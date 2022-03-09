using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TesteBackendEnContact.Core.Domain.ContactBook;
using TesteBackendEnContact.Core.Interface.ContactBook;
using TesteBackendEnContact.Database;
using TesteBackendEnContact.Repository.Interface;

namespace TesteBackendEnContact.Repository
{
    public class ContactBookRepository : IContactBookRepository
    {
        private readonly DatabaseConfig databaseConfig;

        public ContactBookRepository(DatabaseConfig databaseConfig)
        {
            this.databaseConfig = databaseConfig;
        }

        public async Task<IContactBook> SaveAsync(IContactBook contactBook)
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);
            var dao = new ContactBookDao(contactBook);
            
            if(contactBook.Id == 0)
                dao.Id = await connection.InsertAsync(dao);
            else
                await connection.UpdateAsync(dao);

            return dao.Export();
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);
            
            var sql = "DELETE FROM ContactBook WHERE Id = @id";

            await connection.ExecuteAsync(sql, new { id });
        }

        public async Task<IContactBook> GetAsync(int id)
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);

            var query = "SELECT * FROM ContactBook Where Id = @id";
            var result = await connection.QueryAsync<ContactBookDao>(query, new { id });
            
            return result?.FirstOrDefault().Export();
        }

        public async Task<IEnumerable<IContactBook>> GetContactBookByCompanyAsync(int id)
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);

            var query = "SELECT * FROM ContactBook, Company Where Company.ContactBookId = @id";
            var result = await connection.QueryAsync<ContactBookDao>(query, new { id });

            return result?.Select(item => item.Export());
        }

        public async Task<IEnumerable<IContactBook>> GetAllWithFilters(ContactBookFilter filter)
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);

            var query = "";
            IEnumerable<ContactBookDao> result;

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
                    query = @"SELECT * FROM ContactBook 
                        Where Name = @Name ";
                }
                else
                {
                    query = @"SELECT * FROM ContactBook ";
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
                    query = @"SELECT * FROM ContactBook 
                        Where Id = @Id";
                }

                result = await connection.QueryAsync<ContactBookDao>(query, parameters);
                return result?.Select(item => item.Export());
            }

            query = "SELECT * FROM ContactBook LIMIT 10";
            result = await connection.QueryAsync<ContactBookDao>(query);

            return result?.Select(item => item.Export());
        }
    }

    [Table("ContactBook")]
    public class ContactBookDao : IContactBook
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public ContactBookDao()
        {
        }

        public ContactBookDao(IContactBook contactBook)
        {
            Id = contactBook.Id;
            Name = contactBook.Name;
        }

        public IContactBook Export() => new ContactBook(Id, Name);
    }
}
