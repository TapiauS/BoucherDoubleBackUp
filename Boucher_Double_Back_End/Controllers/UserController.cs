
using Boucher_DoubleModel.Models.Entitys;
using Boucher_Double_Back_End.Models.Manager;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Boucher_Double_Back_End.Models.ServerModel;

namespace Boucher_Double_Back_End.Controllers
{
    /// <summary>
    /// Concrete implementation of the abstract <see cref="APIController{User}"/> class
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : APIController<User>
    {
        public UserController(ILogger logger) : base(logger)
        {
        }
        /// <summary>
        /// Generate a json with the success and if needed the CSRF token. If suceeded the session variable associated with the user and its associated store are defined
        /// </summary>
        /// <param name="Login">The login of the user</param>
        /// <param name="Password">The password of the user</param>
        /// <returns>A result containing the connexion attempt result</returns>
        [HttpPost("Connect")]
        public async Task<JsonResult> Connect([FromBody] User userInfo)
        {
            try
            {
                UserDAO userDAO = new();
                User? user = await userDAO.Connect(userInfo.Login, userInfo.Password);
                if (user != null)
                {
                    string csrfToken = Security.GenerateCSRFToken();
                    HttpContext.Session.SetString("UserId", user.IdUser.ToString());
                    HttpContext.Session.SetString("Username", user.Login);
                    HttpContext.Session.SetString("IdStore", user.Store.IdStore.ToString());
                    HttpContext.Session.SetString("CSRF", csrfToken);v
                    HttpContext.Session.SetString("role", user.Role.ToString());
                    AddUserWithExpiration(csrfToken, user, TimeSpan.FromMinutes(60));
                    return new JsonResult(new { success = true, csrf = csrfToken, user = user });
                }
                else
                    return new JsonResult(new { success = false, csrf = "", userInfo.Login });
            }
            catch (DAOException daoe)
            {
                logger.LogError(daoe, "An datatabase error occurred while processing GET request " + daoe.Message);
                throw new Exception(daoe.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"An unknow error occurred while processing GET request "+ex.Message);
                throw new Exception(ex.Message);
            }


        }

        [HttpGet("Disconnect")]

        public async Task Disconnect()
        {
            string key = HttpContext.Request.Headers["X-CSRF-Token"];
            if (ConnectedUser.ContainsKey(key))
            {
                RemoveUser(key);
            }
        }

        [HttpGet("Connected")]

        public async Task<bool> IsConnected()
        {
            return ConnectedUser.ContainsKey(HttpContext.Request.Headers["X-CSRF-Token"]) || HttpContext.Session.GetString("UserId") != null;
        }
    }
}
