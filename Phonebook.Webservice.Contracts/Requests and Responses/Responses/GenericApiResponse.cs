using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Phonebook.Webservice.Contracts.Requests_and_Responses.Responses
{
    public class GenericApiResponse<T>
    {
        public HttpStatusCode Status { get; set; }
        public string Message { get; set; }
        public T Payload { get; set; }

        public static GenericApiResponse<T> Success(T payload)
        {
            return new GenericApiResponse<T>
            {
                Status = HttpStatusCode.OK,
                Message = "Success",
                Payload = payload
            };
        }

        public static GenericApiResponse<T> Failure()
        {
            return new GenericApiResponse<T>
            {
                Status = HttpStatusCode.Forbidden,
                Message = "Failure"
            };
        }
    }
}
