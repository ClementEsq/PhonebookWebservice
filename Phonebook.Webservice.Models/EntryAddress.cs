using Phonebook.Webservice.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phonebook.Webservice.Models
{
    public class EntryAddress
    {
        public string AddressL1 { get; set; }
        public string AddressL2 { get; set; }
        public string PostCode { get; set; }
        public string County { get; set; }
        public string Country { get; set; }
        public AddressType AddressType { get; set; }
        public string EntryType { get; set; }
    }
}
