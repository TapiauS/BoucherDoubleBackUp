using Boucher_DoubleModel.Models.Entitys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Boucher_Double_Back_End.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SelloutController : APIController<Sellout>
    {
        public SelloutController(ILogger logger) : base(logger)
        {
        }
    }
}
