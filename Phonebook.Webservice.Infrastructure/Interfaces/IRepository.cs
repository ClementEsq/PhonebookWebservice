using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phonebook.Webservice.Infrastructure.Interfaces
{
    public interface IRepository<T>
    {
        T GetByIdentifier(string username);
    }
}
