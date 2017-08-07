using System.Threading.Tasks;
using System.Web.Http;
using Phonebook.Webservice.Infrastructure.Interfaces;
using Phonebook.Webservice.Contracts.Requests_and_Responses.Responses;

namespace Phonebook.Webservice.Controllers
{
    [RoutePrefix("phonebook")]
    public class PhonebookController : ApiController
    {
        private readonly IPhonebookService PhonebookService;

        public PhonebookController(IPhonebookService PhonebookService)
        {
            this.PhonebookService = PhonebookService;
        }

        [Route("new")]
        public async Task<IHttpActionResult> CreatePhonebookEntry(PhonebookEntry request)
        {
            if (ModelState.IsValid)
            {
                var response = await PhonebookService.CreatePhonebookEntry(request);

                return Ok(response);
            }

            return BadRequest(ModelState);
        }

        [Route("remove/{id:int:min(1)}")]
        public async Task<IHttpActionResult> DeletePhonebookEntry(int id)
        {
            if (ModelState.IsValid)
            {
                var response = await PhonebookService.DeletePhonebookEntry(id);

                return Ok(response);
            }

            return BadRequest(ModelState);
        }

        [Route("update")]
        public async Task<IHttpActionResult> PatchPhonebookEntry(PhonebookEntry request)
        {
            if (ModelState.IsValid)
            {
                var response = await PhonebookService.EditPhonebookEntry(request);

                return Ok(response);
            }

            return BadRequest(ModelState);
        }

        [Route("entries/{id:int?}")]
        public async Task<IHttpActionResult> GetPhonebook(int? id = null)
        {
            if (ModelState.IsValid)
            {
                var response = await PhonebookService.GetPhonebook(id);

                return Ok(response);
            }

            return BadRequest(ModelState);
        }
    }
}