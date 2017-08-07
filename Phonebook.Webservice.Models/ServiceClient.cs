using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phonebook.Webservice.Models
{
    public class ServiceClient
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int ClientID { get; set; }
    }
}
