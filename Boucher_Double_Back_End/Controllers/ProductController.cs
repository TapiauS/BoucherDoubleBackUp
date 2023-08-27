﻿using Boucher_Double_Back_End.Models.Manager;
using Boucher_DoubleModel.Models.Entitys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using SixLabors.ImageSharp;
using System.Net.Http.Headers;

namespace Boucher_Double_Back_End.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : APIController<Product>
    {
        public ProductController(ILogger logger) : base(logger)
        {
        }

        /// <summary>
        /// Deal with the illustration picture uploading and saving for the <see cref="Product.PicturePath"/> field
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

                        var imagePath = "images/product/" + uniqueFileName;
                        Product product = await DAO.GetByIdAsync(id);
                        if (System.IO.File.Exists("images/product/" + product.PicturePath))
                            System.IO.File.Delete("images/product/" + product.PicturePath);
                        product.PicturePath = uniqueFileName;
                        if (await DAO.UpdateAsync(product))
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
                logger.LogError(daoe, "Error while processing a post request");
                return BadRequest();
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error while processing a post request");
                return BadRequest();
            }
        }

        [HttpGet("image/{pictureName}")]
        public IActionResult GetImage(string pictureName)
        {
            try
            {
                string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string imagePath = Path.Combine(currentDirectory, "images/product", pictureName);

                if (!System.IO.File.Exists(imagePath))
                {
                    return NotFound();
                }

                var fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read);

                return File(fileStream, "image/jpeg");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while processing a get request");
                return BadRequest();
            }
        }
    }
}
