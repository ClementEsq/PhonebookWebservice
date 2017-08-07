using Phonebook.Webservice.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phonebook.Webservice.Models
{
    public class EntryNumber
    {
        public string Number { get; set; }
        public NumberType NumberType { get; set; }
        public string EntryType { get; set; }
    }
}
