using Phonebook.Webservice.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Phonebook.Webservice.Contracts.Requests_and_Responses.Responses;
using Phonebook.Webservice.Models.Connections;
using Serilog;
using Serilog.Sinks.RollingFileAlternate;
using System.Net;
using Dapper;
using System.Data;
using Phonebook.Webservice.Models;

namespace Phonebook.Webservice.Infrastructure
{
    public class PhonebookService : IPhonebookService
    {
        private readonly IPhonebookDbConnection PhonebookDbConnection;
        private static ILogger logger = new LoggerConfiguration().MinimumLevel.Information().WriteTo.RollingFileAlternate("..\\logs\\access", outputTemplate: "{Message}{NewLine}").CreateLogger();

        public PhonebookService(IPhonebookDbConnection PhonebookDbConnection)
        {
            this.PhonebookDbConnection = PhonebookDbConnection;
        }

        public async Task<GenericApiResponse<object>> CreatePhonebookEntry(PhonebookEntry entry)
        {
            GenericApiResponse<object> gResponse = new GenericApiResponse<object>();
            object returnObj = null;

            try
            {
                gResponse.Status = HttpStatusCode.OK;
                gResponse.Message = "Success";

                var parameters = new DynamicParameters();
                parameters.Add("@first_name", entry.Entry.FirstName, dbType: DbType.String);
                parameters.Add("@last_name", entry.Entry.LastName, dbType: DbType.String);
                parameters.Add("@ErrorCode", null, dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@ErrorMessage", null, dbType: DbType.String, direction: ParameterDirection.Output, size: 200);

                var entryId = await PhonebookDbConnection.Connection.QueryFirstOrDefaultAsync<int?>("sp_create_phonebook_entry", parameters, commandType: CommandType.StoredProcedure);

                if (entryId != null)
                {
                    await WriteEntryRecords(entry, (int)entryId);

                    returnObj = new { Id = entryId };
                }
                else
                {
                    gResponse.Message = "Unable to create entry!";
                }
            }
            catch(Exception ex)
            {
                gResponse.Status = HttpStatusCode.InternalServerError;
                gResponse.Message = "Failure";
                logger.Information(ex.Message);
            }

            gResponse.Payload = returnObj;
            return gResponse;
        }

        public async Task<GenericApiResponse<object>> DeletePhonebookEntry(int id)
        {
            GenericApiResponse<object> gResponse = new GenericApiResponse<object>();
            object returnObj = null;

            try
            {
                gResponse.Status = HttpStatusCode.OK;
                gResponse.Message = "Success";

                var parameters = new DynamicParameters();
                parameters.Add("@id", id, dbType: DbType.String);

                await PhonebookDbConnection.Connection.ExecuteAsync("sp_delete_phonebook_entry", parameters, commandType: CommandType.StoredProcedure);

            }
            catch (Exception ex)
            {
                gResponse.Status = HttpStatusCode.InternalServerError;
                gResponse.Message = "Failure";
                logger.Information(ex.Message);
            }

            gResponse.Payload = returnObj;
            return gResponse;
        }

        public async Task<GenericApiResponse<object>> EditPhonebookEntry(PhonebookEntry entry)
        {
            GenericApiResponse<object> gResponse = new GenericApiResponse<object>();
            object returnObj = null;

            try
            {
                gResponse.Status = HttpStatusCode.OK;
                gResponse.Message = "Success";

                var parameters = new DynamicParameters();
                parameters.Add("@entry_id", entry.Entry.Id, dbType: DbType.Int32);
                parameters.Add("@first_name", entry.Entry.FirstName, dbType: DbType.String);
                parameters.Add("@last_name", entry.Entry.LastName, dbType: DbType.String);
                parameters.Add("@error_code", null, dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@error_msg", null, dbType: DbType.String, direction: ParameterDirection.Output, size: 200);

                await PhonebookDbConnection.Connection.ExecuteAsync("sp_update_phonebook_entry", parameters, commandType: CommandType.StoredProcedure);

                var errorOccured = Convert.ToBoolean(parameters.Get<dynamic>("@error_code"));
                var errorMsg = (string)parameters.Get<dynamic>("@error_msg");

                if (!errorOccured)
                {
                    await WriteEntryRecords(entry, (int)entry.Entry.Id);
                }
                else
                {
                    gResponse.Message = "Unable to update entry!";
                }
            }
            catch (Exception ex)
            {
                gResponse.Status = HttpStatusCode.InternalServerError;
                gResponse.Message = "Failure";
                logger.Information(ex.Message);
            }

            gResponse.Payload = returnObj;
            return gResponse;
        }

        public async Task<GenericApiResponse<List<PhonebookEntry>>> GetPhonebook(int? id)
        {
            GenericApiResponse<List<PhonebookEntry>> gResponse = new GenericApiResponse<List<PhonebookEntry>>();
            List<PhonebookEntry> phonebookEntries = new List<PhonebookEntry>();

            try
            {
                gResponse.Status = HttpStatusCode.OK;
                gResponse.Message = "Success";

                var parameters = new DynamicParameters();
                parameters.Add("@id", id, dbType: DbType.Int32);
                parameters.Add("@error_code", null, dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@error_msg", null, dbType: DbType.String, direction: ParameterDirection.Output, size: 200);

                var entries = await PhonebookDbConnection.Connection.QueryAsync<Entry>("sp_get_phonebook_entry", parameters, commandType: CommandType.StoredProcedure);

                var errorOccured = Convert.ToBoolean(parameters.Get<dynamic>("@error_code"));
                var errorMsg = (string)parameters.Get<dynamic>("@error_msg");

                if (!errorOccured && entries != null)
                {
                    var phonebook = await GetEntryRecords(entries.ToList());
                    phonebookEntries = phonebook;
                }
                else
                {
                    gResponse.Message = "Unable to update entry!";
                }
            }
            catch (Exception ex)
            {
                gResponse.Status = HttpStatusCode.InternalServerError;
                gResponse.Message = "Failure";
                logger.Information(ex.Message);
            }

            gResponse.Payload = phonebookEntries;
            return gResponse;
        }

        private async Task<List<PhonebookEntry>> GetEntryRecords(List<Entry> dbEntries)
        {
            List<PhonebookEntry> entries = new List<PhonebookEntry>();

            foreach(var dbEntry in dbEntries)
            {
                var parameters = new DynamicParameters();

                parameters.Add("@entry_id", dbEntry.Id, dbType: DbType.Int32);
                parameters.Add("@error_code", null, dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@error_msg", null, dbType: DbType.String, direction: ParameterDirection.Output, size: 200);
                

                var entryNumbers = await PhonebookDbConnection.Connection.QueryAsync<EntryNumber>("sp_get_entry_numbers", parameters, commandType: CommandType.StoredProcedure);
                var entryEmails = await PhonebookDbConnection.Connection.QueryAsync<EntryEmail>("sp_get_entry_emails", parameters, commandType: CommandType.StoredProcedure);
                var entryAddresses = await PhonebookDbConnection.Connection.QueryAsync<EntryAddress>("sp_get_entry_addresses", parameters, commandType: CommandType.StoredProcedure);

                entries.Add(
                    new PhonebookEntry
                    {
                        Entry = dbEntry,
                        EntryNumbers = entryNumbers.ToList(),
                        EntryEmails = entryEmails.ToList(),
                        EntryAddresses = entryAddresses.ToList()
                    });
            }

            return entries;
        }

        private async Task WriteEntryRecords(PhonebookEntry entry, int entryId)
        {
            var parameters = new DynamicParameters();

            foreach (var entryNumber in entry.EntryNumbers)
            {
                parameters = new DynamicParameters();
                parameters.Add("@entry_id", entryId, dbType: DbType.String);
                parameters.Add("@number", entryNumber.Number, dbType: DbType.String);
                parameters.Add("@number_type_id", entryNumber.NumberType, dbType: DbType.Int32);
                await PhonebookDbConnection.Connection.ExecuteAsync("sp_insert_entry_number", parameters, commandType: CommandType.StoredProcedure);
            }

            foreach (var entryEmail in entry.EntryEmails)
            {
                parameters = new DynamicParameters();
                parameters.Add("@entry_id", entryId, dbType: DbType.String);
                parameters.Add("@email", entryEmail.Email, dbType: DbType.String);
                parameters.Add("@email_type_id", entryEmail.EmailType, dbType: DbType.Int32);
                await PhonebookDbConnection.Connection.ExecuteAsync("sp_insert_entry_number", parameters, commandType: CommandType.StoredProcedure);
            }

            foreach (var entryAddress in entry.EntryAddresses)
            {
                parameters = new DynamicParameters();
                parameters.Add("@entry_id", entryId, dbType: DbType.String);
                parameters.Add("@address_l1", entryAddress.AddressL1, dbType: DbType.String);
                parameters.Add("@address_l2", entryAddress.AddressL2, dbType: DbType.String);
                parameters.Add("@post_code", entryAddress.PostCode, dbType: DbType.String);
                parameters.Add("@county", entryAddress.County, dbType: DbType.String);
                parameters.Add("@country", entryAddress.Country, dbType: DbType.String);
                parameters.Add("@email_type_id", entryAddress.AddressType, dbType: DbType.Int32);
                await PhonebookDbConnection.Connection.ExecuteAsync("sp_insert_entry_address", parameters, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
