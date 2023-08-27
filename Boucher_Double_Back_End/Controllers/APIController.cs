
using Boucher_DoubleModel.Models.Entitys;
using Boucher_Double_Back_End.Models.Manager;
using Microsoft.AspNetCore.Mvc;
using Boucher_DoubleModel.Models;
using System.Runtime.CompilerServices;
using System.Collections.Concurrent;

namespace Boucher_Double_Back_End.Controllers
{
    /// <summary>
    /// Abstract class shared by all the API that define tall the CRUD signature
    /// </summary>
    /// <typeparam name="T">The entity we want to interact through the API</typeparam>
    public class APIController<T> : SessionController
    {
        protected readonly ILogger logger;
        private IDAO<T>? daoInstance;
        /// <summary>
        /// DAO attribute, its concrete implementation is determined by the <see cref="T"/> attribute
        /// </summary>
        public IDAO<T>? DAO
        {
            get
            {
                if (daoInstance == null)
                {
                    switch (typeof(T))
                    {
                        case Type type when type == typeof(BillParameter):
                            daoInstance = new BillParameterDAO() as IDAO<T>;
                            break;
                        case Type type when type == typeof(Category):
                            daoInstance = new CategoryDAO() as IDAO<T>;
                            break;
                        case Type type when type == typeof(MailParameter):
                            daoInstance = new MailParameterDAO() as IDAO<T>;
                            break;
                        case Type type when type == typeof(User):
                            daoInstance = new UserDAO() as IDAO<T>;
                            break;
                        case Type type when type == typeof(Store):
                            daoInstance = new StoreDAO() as IDAO<T>;
                            break;
                        case Type type when type == typeof(Sellout):
                            daoInstance = new SelloutDAO() as IDAO<T>;
                            break;
                        case Type type when type == typeof(Menu):
                            daoInstance = new MenuDAO() as IDAO<T>;
                            break;
                        case Type type when type == typeof(Product):
                            daoInstance = new ProductDAO() as IDAO<T>;
                            break;
                        case Type type when type == typeof(Client):
                            daoInstance = new ClientDAO() as IDAO<T>;
                            break;
                        case Type type when type == typeof(MailContentParameter):
                            daoInstance = new MailContentDAO() as IDAO<T>;
                            break;
                        case Type type when type == typeof(Menu):
                            daoInstance=new MenuDAO() as IDAO<T>; 
                            break;
                        case Type type when type == typeof(Event):
                            daoInstance=new EventDAO() as IDAO<T>;
                            break;
                        default:
                            return null;
                    }
                }
                return daoInstance;
            }
        }



        public APIController(ILogger _logger)
        {
            logger = _logger;
        }

        /// <summary>
        /// Return all the entity if the User is connected using CSRF protection
        /// </summary>
        /// <returns>All the entitys as a Json</returns>
        /// <exception cref="Exception"></exception>
        [HttpGet]
        public async Task<IEnumerable<T>> GetAsync()
        {
            try
            {
                logger.LogInformation("GET request received");
                if (HttpContext.Session.GetString("UserId") != null && DAO != null)
                {
                    DAO.Store = new()
                    {
                        IdStore = int.Parse(HttpContext.Session.GetString("IdStore")),
                        PhoneNumber = ""
                    };
                    DAO.User = new User { IdUser = int.Parse(HttpContext.Session.GetString("UserId")), Role = RoleExtensions.GetRoleFromString(HttpContext.Session.GetString("role")) };
                    return await DAO.GetAllAsync();
                }
                else if (ConnectedUser.ContainsKey(HttpContext.Request.Headers["X-CSRF-Token"]))
                {
                    DAO.Store = ConnectedUser[HttpContext.Request.Headers["X-CSRF-Token"]].Store;
                    DAO.User = ConnectedUser[HttpContext.Request.Headers["X-CSRF-Token"]];
                    return await DAO.GetAllAsync();
                }
                else
                {
                    logger.LogWarning("Invalid session or DAO is null");
                    return default;
                }
            }
            catch (DAOException daoe)
            {
                logger.LogError(daoe, "Error while processing a getall request");
                throw new Exception();
            }
            catch(Exception e)
            {
                logger.LogError(e, "Error while processing a getall request");
                throw new Exception();
            }

        }

        /// <summary>
        /// Return a given entity identified by its id if the User is connected using CSRF protection
        /// </summary>
        /// <param name="id">The entity id</param>
        /// <returns>One entity associated with the request id</returns>
        /// <exception cref="Exception"></exception>
        [HttpGet("{id}")]
        public async Task<T> GetByIdAsync(int id)
        {
            try
            {
                logger.LogInformation("GET request received");
                if (HttpContext.Session.GetString("UserId") != null && DAO != null)
                {
                    DAO.Store = new()
                    {
                        IdStore = int.Parse(HttpContext.Session.GetString("IdStore")),
                    };
                    DAO.User = new User { IdUser = int.Parse(HttpContext.Session.GetString("UserId")), Role = RoleExtensions.GetRoleFromString(HttpContext.Session.GetString("role")) };
                    return await DAO.GetByIdAsync(id);
                }
                else if (ConnectedUser.ContainsKey(HttpContext.Request.Headers["X-CSRF-Token"]))
                {
                    DAO.Store = ConnectedUser[HttpContext.Request.Headers["X-CSRF-Token"]].Store;
                    DAO.User = ConnectedUser[HttpContext.Request.Headers["X-CSRF-Token"]];
                    return await DAO.GetByIdAsync(id);
                }
                else
                {
                    logger.LogError("Invalid session or DAO is null");
                    return default;
                }

            }
            catch (DAOException daoe)
            {
                logger.LogError(daoe, "Error while processing a get request");
                throw new Exception();
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error while processing a get request");
                throw new Exception();
            }
        }
        /// <summary>
        /// Add an entity data to the database if the User is connected using CSRF protection
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost]
        public async Task<int> CreateAsync([FromBody] T value)
        {
            try
            {
                logger.LogInformation("GET request received");
                if (HttpContext.Session.GetString("UserId") != null && DAO != null)
                {
                    DAO.Store = new()
                    {
                        IdStore = int.Parse(HttpContext.Session.GetString("IdStore")),
                    };
                    DAO.User = new User { IdUser = int.Parse(HttpContext.Session.GetString("UserId")), Role = RoleExtensions.GetRoleFromString(HttpContext.Session.GetString("role")) };
                    return await DAO.CreateAsync(value);
                }

                else if (ConnectedUser.ContainsKey(HttpContext.Request.Headers["X-CSRF-Token"]))
                {
                    DAO.Store = ConnectedUser[HttpContext.Request.Headers["X-CSRF-Token"]].Store;
                    DAO.User = ConnectedUser[HttpContext.Request.Headers["X-CSRF-Token"]];
                    return await DAO.CreateAsync(value);
                }
                else
                {
                    logger.LogError("Invalid session or DAO is null");
                    throw new Exception();
                }

            }
            catch (DAOException daoe)
            {
                logger.LogError(daoe, "Error while processing a post request");
                throw new Exception();
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error while processing a post request");
                throw new Exception();
            }
        }
        /// <summary>
        /// Update an entity data in the database if the User is connected using CSRF protection
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="Exception"></exception>
        [HttpPut]
        public async Task<bool> UpdateAsync([FromBody] T value)
        {
            try
            {
                logger.LogInformation("GET request received");
                if (HttpContext.Session.GetString("UserId") != null && DAO != null)
                {
                    DAO.Store = new()
                    {
                        IdStore = int.Parse(HttpContext.Session.GetString("IdStore")),
                    };
                    DAO.User = new User { IdUser = int.Parse(HttpContext.Session.GetString("UserId")), Role = RoleExtensions.GetRoleFromString(HttpContext.Session.GetString("role")) };
                    return await DAO.UpdateAsync(value);
                }

                else if (ConnectedUser.ContainsKey(HttpContext.Request.Headers["X-CSRF-Token"]))
                {
                    DAO.Store = ConnectedUser[HttpContext.Request.Headers["X-CSRF-Token"]].Store;
                    DAO.User = ConnectedUser[HttpContext.Request.Headers["X-CSRF-Token"]];
                    return await DAO.UpdateAsync(value);
                }
                else
                {
                    logger.LogWarning("Invalid session or DAO is null");
                    throw new Exception();
                }

            }
            catch (DAOException daoe)
            {
                logger.LogError(daoe, "Error while processing a put request");
                throw new Exception();
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error while processing a put request");
                throw new Exception();
            }
        }
        /// <summary>
        /// Delete an entity data in the database if the User is connected using CSRF protection
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="Exception"></exception>
        [HttpDelete("{id}")]
        public async Task<bool> DeleteByIdAsync(int id)
        {
            try
            {
                logger.LogInformation("GET request received");
                if (HttpContext.Session.GetString("UserId") != null && DAO != null)
                {
                    DAO.Store = new()
                    {
                        IdStore = int.Parse(HttpContext.Session.GetString("IdStore")),
                    };
                    DAO.User = new User { IdUser = int.Parse(HttpContext.Session.GetString("UserId")), Role = RoleExtensions.GetRoleFromString(HttpContext.Session.GetString("role")) };
                    return await DAO.DeleteAsync(id);
                }

                else if (ConnectedUser.ContainsKey(HttpContext.Request.Headers["X-CSRF-Token"]))
                {
                    DAO.Store = ConnectedUser[HttpContext.Request.Headers["X-CSRF-Token"]].Store;
                    DAO.User = ConnectedUser[HttpContext.Request.Headers["X-CSRF-Token"]];
                    return await DAO.DeleteAsync(id);
                }
                else
                {
                    logger.LogWarning("Invalid session or DAO is null");
                    throw new Exception();
                }

            }
            catch (DAOException daoe)
            {
                logger.LogError(daoe, "Error while processing a delete request");
                throw new Exception();
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error while processing a delete request");
                throw new Exception();
            }
        }
    }
}
