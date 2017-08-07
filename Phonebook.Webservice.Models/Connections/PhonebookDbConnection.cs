using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phonebook.Webservice.Models.Connections
{
    public class PhonebookDbConnection : IPhonebookDbConnection
    {
        public PhonebookDbConnection(IDbConnection connection)
        {
            Connection = connection;
        }

        public IDbConnection Connection { get; }
    }
}
