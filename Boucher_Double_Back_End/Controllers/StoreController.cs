using Boucher_Double_Back_End.Models.Manager;
using Boucher_Double_Back_End.Models.ServerModel;
using Boucher_DoubleModel.Models.Entitys;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using System.Net;
using System.Net.Http.Headers;

namespace Boucher_Double_Back_End.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : APIController<Store>
    {
        public StoreController(ILogger logger) : base(logger)
        {
        }

        /// <summary>
        /// Deal with the illustration picture uploading and saving for the <see cref="Store.LogoPath"/> field
        /// </summary>
        /// <returns></returns>
        [HttpPost("upload/{id}")]
        public async Task<IActionResult> UploadImageAsync([FromForm] IFormFile file, int id)
        {
            try
            {
                if (ConnectedUser.ContainsKey(HttpContext.Request.Headers["X-CSRF-Token"]))
                {
                    DAO.Store = ConnectedUser[HttpContext.Request.Headers["X-CSRF-Token"]].Store;
                    DAO.User = ConnectedUser[HttpContext.Request.Headers["X-CSRF-Token"]];
                    if (file != null)
                    {
                        var fileExtension = Path.GetExtension(ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"'));
                        string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                        string randomString = Path.GetRandomFileName().Replace(".", "");
                        string uniqueFileName = $"{timestamp}_{randomString}{fileExtension}";

                        var imagePath = "images/store/" + uniqueFileName;
                        Store store = await DAO.GetByIdAsync(id);
                        if (System.IO.File.Exists("images/store/" + store.LogoPath))
                            System.IO.File.Delete("images/store/" + store.LogoPath);
                        store.LogoPath = uniqueFileName;
                        if (await DAO.UpdateAsync(store))
                        {

                            using (var image = Image.Load(file.OpenReadStream()))
                            {
                                image.Save(imagePath);
                            }
                            return Ok();
                        }
                        else
                            return BadRequest("Servor error");
                    }
                    else
                        return BadRequest("Invalid file type.");
                }
                else
                    return BadRequest("Not connected");
            }
            catch(DAOException daoe)
            {
                logger.LogError(daoe.Message,daoe);
                return BadRequest();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message,e);
                return BadRequest();
            }
        }


        [HttpGet("image/{pictureName}")]
        public IActionResult GetImage(string pictureName)
        {
            try
            {
                string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string imagePath = Path.Combine(currentDirectory, "images/store", pictureName);

                if (!System.IO.File.Exists(imagePath))
                {
                    return NotFound();
                }

                var fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read);

                return File(fileStream, "image/jpeg");
            }
            catch (Exception ex)
            {
                //TODO logger ici
                throw new Exception(ex.Message, ex);
            }
        }

    }
}
