using Phonebook.Webservice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phonebook.Webservice.Contracts.Requests_and_Responses.Responses
{
    public class PhonebookEntry
    {
        public Entry Entry { get; set; }
        public List<EntryNumber> EntryNumbers { get; set; }
        public List<EntryEmail> EntryEmails { get; set; }
        public List<EntryAddress> EntryAddresses { get; set; }
    }
}
