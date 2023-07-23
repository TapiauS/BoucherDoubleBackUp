using Boucher_DoubleModel.Models.Entitys;
using Microsoft.AspNetCore.Mvc;

namespace Boucher_Double_Back_End.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailContentParameterController:APIController<MailContentParameter>
    {
        public MailContentParameterController(ILogger logger) : base(logger)
        {
        }
    }
}
