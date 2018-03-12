using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text;
using ECommerce.Logging.Data;
using Newtonsoft.Json;

namespace ECommerce.Logging.Api.Controllers
{
    [Route("api/[controller]")]
    public class LoggingController : Controller
    {
        private ILoggingRepository _loggingRepository;

        public LoggingController(ILoggingRepository loggingRepository)
        {
            _loggingRepository = loggingRepository;
        }

        // GET api/logging
        [HttpGet]
        public async Task<IActionResult> Get(string application, string service=null, string server=null)
        {
            var searchResult = await _loggingRepository.SearchAsync(i => i.Application == application && i.Service == service);

            if (searchResult.IsSuccessful && searchResult.Result != null)
            {
                return Ok(searchResult.Result.Where(x => x.Server == server).ToList());
            }

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8, true, 1024, true))
            {
                var bodyString = reader.ReadToEnd();

                if (string.IsNullOrEmpty(bodyString))
                    return BadRequest("Body is empty");


                var loggingItem = JsonConvert.DeserializeObject<LoggingItem>(bodyString);

                if (string.IsNullOrEmpty(loggingItem.Id))
                    loggingItem.Id = Guid.NewGuid().ToString();

                var insertResult = await _loggingRepository.AddAsync(loggingItem);

                return insertResult.IsSuccessful ? (IActionResult)Ok(insertResult.Result) : BadRequest();
            }
        }
    }
}
