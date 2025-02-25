﻿using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesteBackendEnContact.Core.Domain.ContactBook.Company;
using TesteBackendEnContact.Core.Interface.ContactBook.Company;
using TesteBackendEnContact.Database;
using TesteBackendEnContact.Repository.Interface;

namespace TesteBackendEnContact.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly DatabaseConfig databaseConfig;

        public CompanyRepository(DatabaseConfig databaseConfig)
        {
            this.databaseConfig = databaseConfig;
        }

        public async Task<ICompany> SaveAsync(ICompany company)
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);
            var dao = new CompanyDao(company);

            if (dao.Id == 0)
                dao.Id = await connection.InsertAsync(dao);
            else
                await connection.UpdateAsync(dao);

            return dao.Export();
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);
            using var transaction = connection.BeginTransaction();

            var sql = new StringBuilder();
            sql.AppendLine("DELETE FROM Company WHERE Id = @id;");
            sql.AppendLine("UPDATE Contact SET CompanyId = null WHERE CompanyId = @id;");

            await connection.ExecuteAsync(sql.ToString(), new { id }, transaction);
        }

        public async Task<IEnumerable<ICompany>> GetAllWithFiltersAsync(CompanyFilter filter)
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);

            var query = "";
            IEnumerable<CompanyDao> result;

            if (filter != null && (filter.Id > 0 || filter.Page > 0 || filter.ContactBookId > 0 || !string.IsNullOrEmpty(filter.Name)))
            {
                var parameters = new {
                    Id = filter.Id,
                    Name = filter.Name,
                    ContactBookId = filter.ContactBookId,
                    Page = filter.Page
                };

                if(!string.IsNullOrEmpty(filter.Name) || filter.ContactBookId > 0)
                {
                    query = @"SELECT * FROM Company 
                        Where Name = @Name
                        or ContactBookId = @ContactBookId ";
                }
                else
                {
                    query = @"SELECT * FROM Company ";
                }

                if (filter.Page > 0)
                {
                    query += $"LIMIT 10 offset {filter.Page * 10};";
                }
                else
                {
                    query += $"LIMIT  10;";
                }

                if(filter.Id > 0)
                {
                    query = @"SELECT * FROM Company 
                        Where Id = @Id";
                }

                result = await connection.QueryAsync<CompanyDao>(query, parameters);
                return result?.Select(item => item.Export());
            }

            query = "SELECT * FROM Company LIMIT 10";
            result = await connection.QueryAsync<CompanyDao>(query);

            return result?.Select(item => item.Export());
        }

        public async Task<ICompany> GetAsync(int id)
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);

            var query = "SELECT * FROM Company where Id = @id";
            var result = await connection.QuerySingleOrDefaultAsync<CompanyDao>(query, new { id });

            return result?.Export();
        }
    }

    [Table("Company")]
    public class CompanyDao : ICompany
    {
        [Key]
        public int Id { get; set; }

        public int ContactBookId { get; set; }
        public string Name { get; set; }

        public CompanyDao()
        {
        }

        public CompanyDao(ICompany company)
        {
            Id = company.Id;
            ContactBookId = company.ContactBookId;
            Name = company.Name;
        }

        public ICompany Export() => new Company(Id, ContactBookId, Name);
    }
}
