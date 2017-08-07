using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Phonebook.Webservice.Infrastructure.Interfaces;
using Phonebook.Webservice.Models;
using Phonebook.Webservice.Models.Connections;

namespace Phonebook.Webservice.Infrastructure.Repositories
{
    public class ServiceClientRepository : IRepository<ServiceClient>
    {
        private readonly IPhonebookDbConnection PhonebookDbConnection;

        public ServiceClientRepository(IPhonebookDbConnection PhonebookDbConnection)
        {
            this.PhonebookDbConnection = PhonebookDbConnection;
        }

        public ServiceClient GetByIdentifier(string username)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@UserName", username);

                var client = PhonebookDbConnection.Connection.QueryFirstOrDefault<ServiceClient>("sp_select_service_client", parameter, commandType: CommandType.StoredProcedure);

                return client;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
