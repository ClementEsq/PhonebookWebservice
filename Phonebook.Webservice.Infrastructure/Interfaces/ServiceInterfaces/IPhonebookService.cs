using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Phonebook.Webservice.Contracts;
using Phonebook.Webservice.Contracts.Requests_and_Responses.Responses;

namespace Phonebook.Webservice.Infrastructure.Interfaces
{
    public interface IPhonebookService
    {
        Task<GenericApiResponse<object>> CreatePhonebookEntry(PhonebookEntry entry);
        Task<GenericApiResponse<List<PhonebookEntry>>> GetPhonebook(int? id);
        Task<GenericApiResponse<object>> DeletePhonebookEntry(int id);
        Task<GenericApiResponse<object>> EditPhonebookEntry(PhonebookEntry entry);
    }
}
