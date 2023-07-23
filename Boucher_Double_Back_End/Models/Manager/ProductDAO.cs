using Boucher_DoubleModel.Models.Entitys;
using MySqlConnector;

namespace Boucher_Double_Back_End.Models.Manager
{
    /// <summary>
    /// Implementation of the <see cref="IDAO{Product}"/> interface that interact with this associated database entities
    /// </summary>
    public class ProductDAO : IDAO<Product>
    {
        public Store Store { get ; set ; }
        public User User { get ; set ; }

        public async Task<int> CreateAsync(Product entity)
        {
            try
            {
                using MySqlConnection connection = Connexion.getConnexion();

                using MySqlCommand command = connection.CreateCommand();
                command.Connection = connection;
                command.CommandText = "SELECT * FROM product_category WHERE id_store = @idstore AND id = @id";
                command.Parameters.AddWithValue("@idstore", Store.IdStore);
                command.Parameters.AddWithValue("@id", entity.Category.Id);
                using MySqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    using MySqlConnection connection1 = Connexion.getConnexion();

                    using MySqlCommand command1 = connection.CreateCommand();
                    command1.Connection = connection1;
                    command1.CommandText = "INSERT INTO product(name, price, id_product_category,picture_path) VALUES (@name, @price, @id_category,@picture_path) RETURNING id";
                    command1.Parameters.AddWithValue("@name", entity.Name);
                    command1.Parameters.AddWithValue("@price", entity.Price.GetPrice());
                    command1.Parameters.AddWithValue("@id_category", entity.Category.Id);
                    command1.Parameters.AddWithValue("@picture_path", entity.PicturePath);
                    using MySqlDataReader reader1 = command1.ExecuteReader();
                    if(await reader1.ReadAsync())
                        return reader1.GetInt32(0);
                    else
                        return 0;
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
                        return 0;
                        break;
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
        /// Delete a product if its category is know by the <see cref="User"/> store
        /// </summary>
        /// <param name="id">The id of the category to delete</param>
        /// <returns></returns>
        /// <exception cref="DAOException"></exception>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                using MySqlConnection connection = Connexion.getConnexion();

                using MySqlCommand command = connection.CreateCommand();
                command.Connection = connection;
                command.CommandText = "SELECT * FROM category WHERE id = @id AND id_store = @idstore";
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@idstore", Store.IdStore);
                using MySqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    using MySqlConnection connection1 = Connexion.getConnexion();
                    using MySqlCommand command1 = connection.CreateCommand();
                    command1.Connection = connection1;
                    command1.Parameters.Clear();
                    command1.CommandText = "DELETE FROM product WHERE id = @id";
                    command1.Parameters.AddWithValue("@id", id);
                    return await command1.ExecuteNonQueryAsync() > 0;
                }
                else
                    return false;
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
                        break;
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
        /// Return all the product whose category is know by the session shop
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DAOException"></exception>
        public async Task<List<Product>> GetAllAsync()
        {
            try
            {
                using MySqlConnection connection = Connexion.getConnexion();

                using MySqlCommand command = connection.CreateCommand();
                command.Connection = connection;
                command.CommandText = "SELECT * FROM category WHERE id_store = @idstore AND !is_menu(id)";
                command.Parameters.AddWithValue("@idstore", Store.IdStore);
                using MySqlDataReader reader = command.ExecuteReader();
                List<Product> products = new();
                while (await reader.ReadAsync())
                {
                    products.Add(new Product()
                    {
                        Name = reader.GetString(reader.GetOrdinal("name")),
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        Price = new Price(reader.GetDecimal(reader.GetOrdinal("price"))),
                        Category = await new CategoryDAO() { Store=Store,User=User}.GetByIdAsync(reader.GetInt32(reader.GetOrdinal("id_category"))),
                        PicturePath = !reader.IsDBNull(reader.GetOrdinal("picture_path")) ? reader.GetString(reader.GetOrdinal("picture_path")) : ""
                    }); ;
                }
                return products;
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
                        break;
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
    /// Return one the product whose category is know by the session shop and identified by its id
    /// </summary>
    /// <param name="id">The product id</param>
    /// <returns></returns>
    /// <exception cref="DAOException"></exception>
    public async Task<Product> GetByIdAsync(int id)
        {
            try
            {
                using MySqlConnection connection = Connexion.getConnexion();

                using MySqlCommand command = connection.CreateCommand();
                command.Connection = connection;
                command.CommandText = "SELECT * FROM category WHERE id = @id AND id_store = @idstore";
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@idstore", Store.IdStore);
                using MySqlDataReader reader = command.ExecuteReader();
                Product product = new();
                if (await reader.ReadAsync())
                {
                    product = new Product()
                    {
                        Name = reader.GetString(reader.GetOrdinal("name")),
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        Price = new Price(reader.GetDecimal(reader.GetOrdinal("price"))),
                        Category = await new CategoryDAO() { Store = Store, User = User }.GetByIdAsync(reader.GetInt32(reader.GetOrdinal("id_category"))),
                        PicturePath = !reader.IsDBNull(reader.GetOrdinal("picture_path")) ? reader.GetString(reader.GetOrdinal("picture_path")) : ""
                    };
                }
                return product;
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
                        break;
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
        /// Update the given product if its category is associated to the session shop
        /// </summary>
        /// <param name="entity"></param>
        /// <exception cref="DAOException"></exception>
        public async Task<bool> UpdateAsync(Product entity)
        {
            try
            {
                using MySqlConnection connection = Connexion.getConnexion();

                using MySqlCommand command = new();
                command.Connection = connection;
                command.CommandText = "SELECT * FROM product_category WHERE id_store = @idstore AND id = @id";
                command.Parameters.AddWithValue("@idstore", Store.IdStore);
                command.Parameters.AddWithValue("@id", entity.Category.Id);
                using MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    using MySqlConnection connection1 = Connexion.getConnexion();

                    using MySqlCommand command1 = new();
                    command1.Connection = connection1;
                    command1.CommandText = "UPDATE product SET name = @name, price = @price, id_product_category = @id_category, picture_path=@picture_path  WHERE id = @id";
                    command1.Parameters.AddWithValue("@name", entity.Name);
                    command1.Parameters.AddWithValue("@price", entity.Price.GetPrice());
                    command1.Parameters.AddWithValue("@id_category", entity.Category.Id);
                    command1.Parameters.AddWithValue("@id", entity.Id);
                    command1.Parameters.AddWithValue("@picture_path", entity.PicturePath!=""?entity.PicturePath:null);
                    return await command1.ExecuteNonQueryAsync() > 0;
                }
                else
                    return false;

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
                        break;
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
