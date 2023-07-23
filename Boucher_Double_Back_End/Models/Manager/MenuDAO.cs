using Boucher_DoubleModel.Models.Entitys;
using MySqlConnector;
using System.Diagnostics.Eventing.Reader;
using System.Xml.Linq;

namespace Boucher_Double_Back_End.Models.Manager
{
    /// <summary>
    /// Implementation of the <see cref="IDAO{Menu}"/> interface that interact with this associated database entities
    /// </summary>
    public class MenuDAO : IDAO<Menu>
    {
        public Store Store { get; set; }
        public User User { get; set; }

        public async Task<int> CreateAsync(Menu entity)
        {
            try
            {
                using MySqlConnection verifConn=Connexion.getConnexion();
                using MySqlCommand testCommand = new();
                testCommand.Connection = verifConn;
                testCommand.CommandText = "SELECT * FROM product_category WHERE id=@idcategory AND id_store=@idstore";
                testCommand.Parameters.AddWithValue("@idcategory", entity.Category.Id);
                testCommand.Parameters.AddWithValue("@idstore", Store.IdStore);
                if(testCommand.ExecuteReader().Read()) 
                {
                    using MySqlConnection connection = Connexion.getConnexion();
                    using MySqlCommand command = new();
                    command.Connection = connection;
                    command.CommandText = "INSERT INTO product(name, id_product_category, price)" +
                                            "VALUES(@name, @idcategory, @price)" +
                                            "RETURNING id";
                    command.Parameters.AddWithValue("@name", entity.Name);
                    command.Parameters.AddWithValue("@idstore", Store.Id);
                    command.Parameters.AddWithValue("@idcategory", entity.Category.Id);
                    command.Parameters.AddWithValue("@price", entity.Price.GetPrice());
                    using MySqlDataReader reader = command.ExecuteReader();
                    if (await reader.ReadAsync())
                    {
                        int id = reader.GetInt32(0);
                        Console.WriteLine(id);
                        foreach (SelloutLine item in entity.Content)
                        {
                            using MySqlConnection connection1 = Connexion.getConnexion();

                            using MySqlCommand command1 = new();
                            command1.Connection = connection1;
                            command1.CommandText = "INSERT INTO contain(id_product, id_menu, quantity) " +
                                                  "VALUES (@idproduct, @idmenu, @quantity)";
                            command1.Parameters.AddWithValue("@idmenu", id);
                            command1.Parameters.AddWithValue("@idproduct", item.SoldProduct.Id);
                            command1.Parameters.AddWithValue("@quantity", item.Quantity);

                            if (await command1.ExecuteNonQueryAsync() <= 0)
                                return 0;
                        }

                        return id;
                    }
                    else
                    {
                        return 0;
                    }
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

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                using MySqlConnection connection = Connexion.getConnexion();

                using MySqlCommand command = new ();
                command.Connection = connection;
                command.CommandText = "DELETE FROM product WHERE product.id = @id AND (SELECT id_store FROM menu WHERE menu.id_menu = @id) = @idstore";
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

        public async Task<List<Menu>> GetAllAsync()
        {
            try
            {
                using MySqlConnection connection = Connexion.getConnexion();

                using MySqlCommand command = new ();
                command.Connection = connection;
                command.CommandText = "SELECT * FROM menu WHERE id_store = @idstore";
                command.Parameters.AddWithValue("@idstore", Store.IdStore);
                List<Menu> list = new ();

                using MySqlDataReader reader = command.ExecuteReader();
                int id = 0;
                string name = "";
                decimal price = 0;
                Category category = null;
                List<SelloutLine> products = new();

                while (await reader.ReadAsync())
                {
                    if (category == null)
                    {
                        int categoryId = reader.GetInt32(reader.GetOrdinal("id_product_category"));
                        category = await new CategoryDAO() {User=User,Store=Store }.GetByIdAsync(categoryId);
                        id = reader.GetInt32(reader.GetOrdinal("id_menu"));
                        name = reader.GetString(reader.GetOrdinal("menu_name"));
                        price = reader.GetDecimal(reader.GetOrdinal("price"));
                        int productId = reader.GetInt32(reader.GetOrdinal("id"));
                        products.Add(new() { SoldProduct = await new ProductDAO() { User = User, Store = Store }.GetByIdAsync(productId), Quantity = reader.GetInt32(reader.GetOrdinal("quantity")) });
                    }
                    else if (id == reader.GetInt32(reader.GetOrdinal("id")) || id == 0)
                    {
                        int productId = reader.GetInt32(reader.GetOrdinal("id"));
                        products.Add(new() { SoldProduct = await new ProductDAO() { User = User, Store = Store }.GetByIdAsync(productId), Quantity = reader.GetInt32(reader.GetOrdinal("quantity")) });
                    }
                    else
                    {
                        list.Add(new Menu
                        {
                            Id = id,
                            Name = name,
                            Price = new Price(price),
                            Category = category,
                            Content = new (products)
                        });

                        id = 0;
                        name = "";
                        price = 0;
                        category = null;
                        products = new ();
                    }
                }

                if (id != 0)
                {
                    list.Add(new Menu
                    {
                        Id = id,
                        Name = name,
                        Price = new Price(price),
                        Category = category,
                        Content = new (products)
                    });
                }

                return list;
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

        public async Task<Menu> GetByIdAsync(int id)
        {
            try
            {
                using MySqlConnection connexion = Connexion.getConnexion();
                
                using MySqlCommand command = new();
                command.Connection = connexion;
                command.CommandText = "SELECT * FROM menu WHERE id_store=@idstore AND id_menu=@id";
                command.Parameters.AddWithValue("@idstore", Store.IdStore);
                command.Parameters.AddWithValue("@id", id);
                using MySqlDataReader reader = command.ExecuteReader();
                int id_menu = 0;
                string name = "";
                decimal price = 0;
                Category category = null;
                List<SelloutLine> products = new();

                while (await reader.ReadAsync())
                {
                    if (category == null)
                    {
                        category = await new CategoryDAO() { User = User, Store = Store }.GetByIdAsync(reader.GetInt32(reader.GetOrdinal("id_product_category")));
                        id_menu = reader.GetInt32(reader.GetOrdinal("id_menu"));
                        name = reader.GetString(reader.GetOrdinal("menu_name"));
                        price = reader.GetDecimal(reader.GetOrdinal("price"));
                        products.Add(new() { SoldProduct = await new ProductDAO() { User = User, Store = Store }.GetByIdAsync(id), Quantity = reader.GetInt32(reader.GetOrdinal("quantity")) });
                    }

                    if (id_menu == reader.GetInt32(reader.GetOrdinal("id")) || id_menu == 0)
                    {
                        products.Add(new() { SoldProduct = await new ProductDAO() { User = User, Store = Store }.GetByIdAsync(id), Quantity = reader.GetInt32(reader.GetOrdinal("quantity")) });
                    }
                }
                return new Menu
                {
                    Id = id,
                    Name = name,
                    Price = new Price(price),
                    Category = category,
                    Content = products
                };
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

        public async Task<bool> UpdateAsync(Menu entity)
        {
            try
            {
                using MySqlConnection connection = Connexion.getConnexion();

                using MySqlCommand command = new MySqlCommand();
                command.Connection = connection;
                command.CommandText = "UPDATE product SET name = @name, id_product_category = @idcategory, price = @price WHERE (SELECT id_store FROM product_category WHERE id = @idcategory) = @idstore AND id = @id";
                command.Parameters.AddWithValue("@name", entity.Name);
                command.Parameters.AddWithValue("@id", entity.Id);
                command.Parameters.AddWithValue("@idcategory", entity.Category.Id);
                command.Parameters.AddWithValue("@idstore", Store.IdStore);
                command.Parameters.AddWithValue("@price", entity.Price.GetPrice());
                if(await command.ExecuteNonQueryAsync() > 0)
                {
                    using MySqlConnection connection1 = Connexion.getConnexion();

                    using MySqlCommand command1 = new ();
                    command1.Connection = connection1;
                    command1.CommandText = "DELETE FROM contain WHERE id_menu = @idmenu";
                    command1.Parameters.Clear();
                    command1.Parameters.AddWithValue("@idmenu", entity.Id);
                    if (await command1.ExecuteNonQueryAsync() > 0)
                    {
                        foreach (SelloutLine item in entity.Content)
                        {
                            using MySqlConnection connection2 = Connexion.getConnexion();

                            using MySqlCommand command2 = new();
                            command2.Connection = connection2;
                            command2.Parameters.Clear();
                            command2.CommandText = "INSERT INTO contain(id_product, id_menu, quantity) VALUES (@idproduct, @idmenu, @quantity)";
                            command2.Parameters.AddWithValue("@idmenu", entity.Id);
                            command2.Parameters.AddWithValue("@idproduct", item.SoldProduct.Id);
                            command2.Parameters.AddWithValue("@quantity", item.Quantity);
                            if (await command2.ExecuteNonQueryAsync() <= 0)
                                return false;
                        }
                        return true;
                    }
                    else
                        return false;
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
