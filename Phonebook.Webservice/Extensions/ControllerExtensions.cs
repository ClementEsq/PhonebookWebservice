using System.Web.Http;
using System.Web.Http.Results;
using Phonebook.Webservice.Contracts;
using Phonebook.Webservice.Contracts.Requests_and_Responses.Responses;

namespace Phonebook.Webservice.Extensions
{
    public static class ControllerExtensions
    {
        public static IHttpActionResult GetResponse<T>(this ApiController @this, T response)
        {
            if (response != null)
            {
                return new OkNegotiatedContentResult<GenericApiResponse<T>>(GenericApiResponse<T>.Success(response), @this);
            }

            return new OkNegotiatedContentResult<GenericApiResponse<T>>(GenericApiResponse<T>.Failure(), @this);
        }
    }
}
