using Boucher_DoubleModel.Models.Entitys;
using MySqlConnector;

namespace Boucher_Double_Back_End.Models.Manager
{
    /// <summary>
    /// Implementation of the <see cref="IDAO{Category}"/> interface that intercat with this associated database entities
    /// </summary>
    public class CategoryDAO : IDAO<Category>
    {
        public Store Store { get ; set; }
        public User User { get; set; }
        
        public async Task<int> CreateAsync(Category entity)
        {
            try
            {
                using MySqlConnection connexion = Connexion.getConnexion();
                
                using MySqlCommand command = new();
                command.Connection = connexion;
                command.CommandText = "INSERT INTO product_category(name,id_store,id_container,picture_path) VALUES (@name,@idstore,@idcontainer,@picture_path) RETURNING id";
                command.Parameters.AddWithValue("@name", entity.Name);
                command.Parameters.AddWithValue("@idstore", Store.IdStore);
                command.Parameters.AddWithValue("@picture_path", entity.PicturePath);
                if(entity.IdContainer!=0)
                    command.Parameters.AddWithValue("@idcontainer", entity.IdContainer);
                else
                    command.Parameters.AddWithValue("@idcontainer", null);
                using MySqlDataReader reader = command.ExecuteReader();
                if (await reader.ReadAsync())
                {
                    return reader.GetInt32(0);
                }
                else
                    return 0;
            }
            catch (IOException ioe)
            {
                throw new DAOException("Erreur de connexion", ioe, ErrorTypeDAO.IOE);
            }
            catch (MySqlException msqe)
            {
                switch (msqe.ErrorCode)
                {
                    case MySqlErrorCode.DuplicateKeyEntry:
                        return default;
                        
                    default:
                        throw new DAOException("Erreur SQL grave", msqe, ErrorTypeDAO.SQLSEVERE);
                }
            }
            catch (Exception e)
            {
                throw new DAOException("Erreur inconnue", e, ErrorTypeDAO.UNKNOW);
            }

        }
        /// <summary>
        /// Delete the associated category if its associated with the session store
        /// </summary>
        /// <param name="id">id of the <see cref="Category"/></param>
        /// <returns></returns>
        /// <exception cref="DAOException"></exception>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                using MySqlConnection connexion = Connexion.getConnexion();
                
                using MySqlCommand command = new();
                command.Connection = connexion;
                command.CommandText = "DELETE FROM product_category WHERE id=@id AND id_store=@idstore";
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@idstore", Store.IdStore);
                return await command.ExecuteNonQueryAsync() > 0;
            }
            catch (IOException ioe)
            {
                throw new DAOException("Erreur de connexion", ioe, ErrorTypeDAO.IOE);
            }
            catch (MySqlException msqe)
            {
                switch (msqe.ErrorCode)
                {
                    case MySqlErrorCode.DuplicateKeyEntry:
                        return false;
                        
                    default:
                        throw new DAOException("Erreur SQL grave", msqe, ErrorTypeDAO.SQLSEVERE);
                }
            }
            catch (Exception e)
            {
                throw new DAOException("Erreur inconnue", e, ErrorTypeDAO.UNKNOW);
            }
        }

        public async Task<List<Category>> GetAllAsync()
        {
            try
            {
                using MySqlConnection connexion = Connexion.getConnexion();
                
                using MySqlCommand command = new();
                command.Connection = connexion;
                command.CommandText = "SELECT * FROM product_category WHERE id_store=@idstore AND id_container IS NULL";
                command.Parameters.AddWithValue("@idstore", Store.IdStore);
                using MySqlDataReader reader = command.ExecuteReader();
                List<Category> result = new ();
                while (await reader.ReadAsync())
                {
                    result.Add(new Category()
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        Name = reader.GetString(reader.GetOrdinal("name")),
                        SubCategory = await GetSubcategoryAsync(reader.GetInt32(reader.GetOrdinal("id"))),
                        IdContainer = !reader.IsDBNull(reader.GetOrdinal("id_container")) ? reader.GetInt32(reader.GetOrdinal("id_container")) : 0,
                        Store = Store,
                        PicturePath = !reader.IsDBNull(reader.GetOrdinal("picture_path")) ? reader.GetString(reader.GetOrdinal("picture_path")) : ""
                    }) ;
                }
                return result;
            }
            catch (IOException ioe)
            {
                throw new DAOException("Erreur de connexion", ioe, ErrorTypeDAO.IOE);
            }
            catch (MySqlException msqe)
            {
                switch (msqe.ErrorCode)
                {
                    case MySqlErrorCode.DuplicateKeyEntry:
                        return default;
                        
                    default:
                        throw new DAOException("Erreur SQL grave", msqe, ErrorTypeDAO.SQLSEVERE);
                }
            }
            catch (Exception e)
            {
                throw new DAOException("Erreur inconnue", e, ErrorTypeDAO.UNKNOW);
            }
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            try
            {
                using MySqlConnection connexion = Connexion.getConnexion();
                using MySqlCommand command = new();
                command.Connection = connexion;
                command.CommandText = "SELECT * FROM product_category WHERE id=@id AND id_store=@idstore";
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@idstore", Store.IdStore);
                using MySqlDataReader reader = command.ExecuteReader();
                Category result = new();
                if (await reader.ReadAsync())
                {
                    result.Name = reader.GetString(reader.GetOrdinal("name"));
                    result.Id = reader.GetInt32(reader.GetOrdinal("id"));
                    result.SubCategory = await GetSubcategoryAsync(reader.GetInt32(reader.GetOrdinal("id")));
                    result.Store = Store;
                    result.PicturePath = !reader.IsDBNull(reader.GetOrdinal("picture_path")) ? reader.GetString(reader.GetOrdinal("picture_path")) : "";
                }
                return result;
            }
            catch (IOException ioe)
            {
                throw new DAOException("Erreur de connexion", ioe, ErrorTypeDAO.IOE);
            }
            catch (MySqlException msqe)
            {
                switch (msqe.ErrorCode)
                {
                    case MySqlErrorCode.DuplicateKeyEntry:
                        return default;
                        
                    default:
                        throw new DAOException("Erreur SQL grave", msqe, ErrorTypeDAO.SQLSEVERE);
                }
            }
            catch (Exception e)
            {
                throw new DAOException("Erreur inconnue", e, ErrorTypeDAO.UNKNOW);
            }
        }

        public async Task<bool> UpdateAsync(Category entity)
        {
            try
            {
                using MySqlConnection connexion = Connexion.getConnexion();
                
                using MySqlCommand command = new();
                command.Connection = connexion;
                command.CommandText = "UPDATE product_category SET name=@name,picture_path=@picture_path WHERE id=@id AND id_store=@idstore";
                command.Parameters.AddWithValue("@id", entity.Id);
                command.Parameters.AddWithValue("@picture_path", entity.PicturePath);
                command.Parameters.AddWithValue("@idstore", Store.IdStore);
                command.Parameters.AddWithValue("@name", entity.Name);
                return await command.ExecuteNonQueryAsync()> 0;
            }
            catch (IOException ioe)
            {
                throw new DAOException("Erreur de connexion", ioe, ErrorTypeDAO.IOE);
            }
            catch (MySqlException msqe)
            {
                switch (msqe.ErrorCode)
                {
                    case MySqlErrorCode.DuplicateKeyEntry:
                        return false;
                        
                    default:
                        throw new DAOException("Erreur SQL grave", msqe, ErrorTypeDAO.SQLSEVERE);
                }
            }
            catch (Exception e)
            {
                throw new DAOException("Erreur inconnue", e, ErrorTypeDAO.UNKNOW);
            }
        }

        public async Task<List<Category>> GetSubcategoryAsync(int id)
        {
            try
            {
                using MySqlConnection connexion = Connexion.getConnexion();
                using MySqlCommand command = new();
                command.Connection = connexion;
                command.CommandText = "SELECT * FROM product_category WHERE id_container=@id AND id_store=@idstore";
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@idstore", Store.IdStore);
                using MySqlDataReader reader = command.ExecuteReader();
                List<Category> result = new();
                while (await reader.ReadAsync())
                {
                    result.Add(new() { Id = reader.GetInt32(reader.GetOrdinal("id")), Name = reader.GetString(reader.GetOrdinal("name")), SubCategory=await GetSubcategoryAsync(reader.GetInt32(reader.GetOrdinal("id"))),IdContainer=id,Store=Store });
                }
                return result;
            }
            catch (IOException ioe)
            {
                throw new DAOException("Erreur de connexion", ioe, ErrorTypeDAO.IOE);
            }
            catch (MySqlException msqe)
            {
                switch (msqe.ErrorCode)
                {
                    case MySqlErrorCode.DuplicateKeyEntry:
                        return default;
                        
                    default:
                        throw new DAOException("Erreur SQL grave", msqe, ErrorTypeDAO.SQLSEVERE);
                }
            }
            catch (Exception e)
            {
                throw new DAOException("Erreur inconnue", e, ErrorTypeDAO.UNKNOW);
            }
        }
    }
}
