using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phonebook.Webservice.Models.Connections
{
    public interface IPhonebookDbConnection
    {
        IDbConnection Connection { get; }
    }
}
