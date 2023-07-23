using Boucher_DoubleModel.Models.Entitys;
using Microsoft.AspNetCore.Mvc;

namespace Boucher_Double_Back_End.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController:APIController<Client>
    {
        public ClientController(ILogger logger) : base(logger)
        {
        }
    }
}
