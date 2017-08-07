using Phonebook.Webservice.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phonebook.Webservice.Models
{
    public class EntryEmail
    {
        public string Email { get; set; }
        public EmailType EmailType { get; set; }
        public string EntryType { get; set; }
    }
}
