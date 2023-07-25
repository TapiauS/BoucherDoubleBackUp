using Boucher_DoubleModel.Models.Entitys;
using Microsoft.AspNetCore.Mvc;

namespace Boucher_Double_Back_End.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : APIController<Event>
    {
        public EventController(ILogger _logger) : base(_logger)
        {
        }
    }
}
