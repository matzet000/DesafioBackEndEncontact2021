﻿using System.ComponentModel.DataAnnotations;
using TesteBackendEnContact.Core.Domain.ContactBook;
using TesteBackendEnContact.Core.Interface.ContactBook;

namespace TesteBackendEnContact.Controllers.Models
{
    public class SaveContactBookRequest
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public IContactBook ToContactBook() => new ContactBook(Id, Name);
    }
}
