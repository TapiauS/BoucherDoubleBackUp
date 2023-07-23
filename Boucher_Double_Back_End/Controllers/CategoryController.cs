using Boucher_DoubleModel.Models.Entitys;
using Boucher_Double_Back_End.Models.Manager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Boucher_DoubleModel.Models;
using System.Net.Http.Headers;

namespace Boucher_Double_Back_End.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : APIController<Category>
    {
        public CategoryController(ILogger logger) : base(logger)
        {
        }


        /// <summary>
        /// Deal with the illustration picture uploading and saving for the <see cref="Category.PicturePath"/> field
        /// </summary>
        /// <returns></returns>
        [HttpPost("upload/{id}")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file, int id)
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

                        var imagePath = "images/category/" + uniqueFileName;
                        Category category = await DAO.GetByIdAsync(id);
                        if (System.IO.File.Exists("images/category/" + category.PicturePath))
                            System.IO.File.Delete("images/category/" + category.PicturePath);
                        category.PicturePath = uniqueFileName;
                        if (await DAO.UpdateAsync(category))
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
            catch (DAOException daoe)
            {
                logger.LogError(daoe.Message, daoe);
                return BadRequest();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message, e);
                return BadRequest();
            }

        }

        [HttpGet("image/{pictureName}")]
        public IActionResult GetImage(string pictureName)
        {
            try
            {
                string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string imagePath = Path.Combine(currentDirectory, "images/category", pictureName);

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


        [HttpGet("SubCategory/{id}")]
        public async Task<List<Category>> GetSubCategory(int id)
        {
            try
            {
                if (HttpContext.Session.GetString("UserId") != null && DAO != null)
                {
                    CategoryDAO categoryDao = new()
                    {
                        Store = new()
                        {
                            IdStore = int.Parse(HttpContext.Session.GetString("IdStore")),
                        },
                        User = new User { IdUser = int.Parse(HttpContext.Session.GetString("UserId")), Role = RoleExtensions.GetRoleFromString(HttpContext.Session.GetString("role")) }
                    };
                    return await categoryDao.GetSubcategoryAsync(id);
                }
                else if (ConnectedUser.ContainsKey(HttpContext.Request.Headers["X-CSRF-Token"]))
                {
                    CategoryDAO categoryDao = new()
                    {
                        Store = ConnectedUser[HttpContext.Request.Headers["X-CSRF-Token"]].Store,
                        User = ConnectedUser[HttpContext.Request.Headers["X-CSRF-Token"]]
                    };
                    return await categoryDao.GetSubcategoryAsync(id);
                }
                else
                {
                    logger.LogWarning("Invalid session or DAO is null");
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
