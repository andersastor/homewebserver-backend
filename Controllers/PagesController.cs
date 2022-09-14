using homewebserver_backend.Models;
using homewebserver_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace homewebserver_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PagesController : ControllerBase
    {
        private readonly ILogger<PagesController> _logger;

        private JsonFilePageService PageService { get; }

        public PagesController(JsonFilePageService jsonFilePageService, ILogger<PagesController> logger)
        {
            PageService = jsonFilePageService;
            _logger = logger;
        }


        [HttpGet(Name = "getPages")]
        public IEnumerable<Page> Get() => PageService.GetPages();
    }
}
