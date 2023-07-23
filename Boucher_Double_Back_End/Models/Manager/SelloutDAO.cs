
using Boucher_DoubleModel.Models.Entitys;
using MySqlConnector;

namespace Boucher_Double_Back_End.Models.Manager
{
    /// <summary>
    /// Implementation of the <see cref="IDAO{Sellout}"/> interface that interact with this associated database entities 
    /// </summary>
    public class SelloutDAO : IDAO<Sellout>
    {
        public Store Store { get ; set ; }
        public User User { get ; set ; }

        public async Task<int> CreateAsync(Sellout entity)
        {
            try
            {
                using MySqlConnection connexion = Connexion.getConnexion();
                
                using MySqlCommand command = new();
                command.Connection = connexion;
                command.CommandText = "INSERT INTO sellout(id_person,receipt_date,livraison_date) VALUES (@id_person,@receipt_date,@sellout_date) RETURNING id";
                command.Parameters.AddWithValue("@id_person", entity.Client.Id);
                command.Parameters.AddWithValue("@receipt_date", entity.ReceiptDate);
                command.Parameters.AddWithValue("@sellout_date", entity.SelloutDate);
                using MySqlDataReader reader = command.ExecuteReader();
                if (await reader.ReadAsync())
                {
                    int id = reader.GetInt32(reader.GetOrdinal("id"));
                    foreach (SelloutLine line in entity.BuyedProducts)
                    {
                        using MySqlConnection connexion1 = Connexion.getConnexion();

                        using MySqlCommand command1 = new();
                        command1.Connection = connexion1;
                        command1.CommandText = "INSERT INTO concern(id_sellout,id_product,quantity) VALUES (@id_sellout,(SELECT id FROM product WHERE id=@id_product AND (SELECT id_store FROM product_category WHERE id=@idcategory)=@idstore),@quantity)";
                        command1.Parameters.AddWithValue("@id_sellout", id);
                        command1.Parameters.AddWithValue("@id_product", line.SoldProduct.Id);
                        command1.Parameters.AddWithValue("@idcategory",line.SoldProduct.Category.Id);
                        command1.Parameters.AddWithValue("@idstore", Store.IdStore);
                        command1.Parameters.AddWithValue("@quantity", line.Quantity);
                        if (await command1.ExecuteNonQueryAsync() <= 0)
                            return 0;
                    }
                    return id;
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
                using MySqlConnection connexion = Connexion.getConnexion();

                using MySqlCommand command = new();
                command.Connection = connexion;
                command.CommandText = "SELECT id FROM fullsellout WHERE id_store=@idstore AND id=@id";
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@idstore", Store.IdStore);
                using MySqlDataReader reader = command.ExecuteReader();
                if (await reader.ReadAsync())
                {
                    using MySqlConnection connexion1 = Connexion.getConnexion();
                    using MySqlCommand command1 = new();
                    command1.Connection = connexion1;
                    command1.CommandText = "DELETE FROM sellout WHERE id=@id";
                    command1.Parameters.AddWithValue("@id", id);
                    return await command1.ExecuteNonQueryAsync() > 0;
                }
                else
                {
                    return false;
                }
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

        public async Task<List<Sellout>> GetAllAsync()
        {
            try
            {
                using MySqlConnection connexion = Connexion.getConnexion();

                using MySqlCommand command = new();
                command.Connection = connexion;
                command.CommandText = "SELECT * FROM fullsellout WHERE id_store=@idstore";
                command.Parameters.AddWithValue("@idstore", Store.IdStore);
                using MySqlDataReader reader = command.ExecuteReader();
                List<Sellout> sellouts = new();
                int id = 0;
                int id_person = 0;
                DateTime receipt_date = DateTime.Now;
                DateTime sellout_date = DateTime.Now;
                List<SelloutLine> content = new();
                while (await reader.ReadAsync())
                {
                    if (reader.GetInt32(reader.GetOrdinal("id")) == id||id==0)
                    {
                        id = reader.GetInt32(reader.GetOrdinal("id"));
                        id_person = reader.GetInt32(reader.GetOrdinal("id_person"));
                        receipt_date = reader.GetDateTime(reader.GetOrdinal("receipt_date"));
                        sellout_date = reader.GetDateTime(reader.GetOrdinal("livraison_date"));
                        content.Add(new() { SoldProduct = await new ProductDAO() { User = User, Store = Store }.GetByIdAsync(reader.GetInt32(reader.GetOrdinal("id_product"))), Quantity = reader.GetInt32(reader.GetOrdinal("quantity"))});
                    }
                    else
                    {
                        sellouts.Add(new Sellout
                        {
                            Id = id,
                            ReceiptDate = receipt_date,
                            SelloutDate = sellout_date,
                            BuyedProducts = content,
                            Client = await new ClientDAO() { Store = Store, User = User }.GetByIdAsync(id_person),
                            Store = Store
                        });
                        id = reader.GetInt32(reader.GetOrdinal("id"));
                        id_person = reader.GetInt32(reader.GetOrdinal("id_person"));
                        receipt_date = reader.GetDateTime(reader.GetOrdinal("receipt_date"));
                        sellout_date = reader.GetDateTime(reader.GetOrdinal("livraison_date"));
                        content = new ()
                        {
                            { new() { SoldProduct = await new ProductDAO() { User = User, Store = Store }.GetByIdAsync(reader.GetInt32(reader.GetOrdinal("id_product"))), Quantity = reader.GetInt32(reader.GetOrdinal("quantity"))} }
                        };
                    }
                }
                sellouts.Add(new Sellout
                {
                    Id = id,
                    ReceiptDate = receipt_date,
                    SelloutDate = sellout_date,
                    BuyedProducts = content,
                    Client = await new ClientDAO() { Store = Store, User = User }.GetByIdAsync(id_person)
                });
                return sellouts;
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


        public async Task<Sellout> GetByIdAsync(int id)
        {
            try
            {
                using MySqlConnection connexion = Connexion.getConnexion();

                using MySqlCommand command = new();
                command.Connection = connexion;
                command.CommandText = "SELECT * FROM fullsellout WHERE id=@id AND id_store=@idstore";
                command.Parameters.AddWithValue("@idstore", Store.IdStore);
                command.Parameters.AddWithValue("@id", id);
                using MySqlDataReader reader = command.ExecuteReader();
                Sellout sellout = new ();

                int id_person = 0;
                DateTime receipt_date = DateTime.Now;
                DateTime sellout_date = DateTime.Now;
                List<SelloutLine> content = new();
                while (await reader.ReadAsync())
                {
                    id = reader.GetInt32(reader.GetOrdinal("id"));
                    id_person = reader.GetInt32(reader.GetOrdinal("id_person"));
                    receipt_date = reader.GetDateTime(reader.GetOrdinal("receipt_date"));
                    sellout_date = reader.GetDateTime(reader.GetOrdinal("livraison_date"));
                    content.Add(new() { SoldProduct = await new ProductDAO() { User = User, Store = Store }.GetByIdAsync(reader.GetInt32(reader.GetOrdinal("id_product"))), Quantity = reader.GetInt32(reader.GetOrdinal("quantity")) });
                }
                sellout = new Sellout
                {
                    Id = id,
                    ReceiptDate = receipt_date,
                    SelloutDate = sellout_date,
                    BuyedProducts = content,
                    Client = await new ClientDAO() { Store = Store, User = User }.GetByIdAsync(id_person),
                    Store = Store
                };
                return sellout;
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

        public async Task<bool> UpdateAsync(Sellout entity)
        {
            try
            {
                using MySqlConnection connexion = Connexion.getConnexion();
                using MySqlCommand command = new();
                command.Connection = connexion;
                command.CommandText = "UPDATE sellout SET id_person=@id_person, receipt_date=@receipt_date, livraison_date=@livraison_date WHERE id=@id AND (SELECT id FROM full_store WHERE id_store=@id_store)=@id_person";
                command.Parameters.AddWithValue("@id_person", entity.Client.Id);
                command.Parameters.AddWithValue("@receipt_date", entity.ReceiptDate);
                command.Parameters.AddWithValue("@livraison_date", entity.SelloutDate);
                command.Parameters.AddWithValue("@id_store", Store.IdStore);
                command.Parameters.AddWithValue("@id", entity.Id);
                if (await command.ExecuteNonQueryAsync()>0)
                {
                    foreach (SelloutLine line in entity.BuyedProducts)
                    {
                        Product product = await new ProductDAO() { User = User, Store = Store }.GetByIdAsync(line.SoldProduct.Id) ;
                        if (product.Category.Store.IdStore == Store.IdStore)
                        {
                            using MySqlConnection connexion1 = Connexion.getConnexion();
                            using MySqlCommand command1 = new();
                            command1.Connection = connexion1;
                            command1.CommandText = "UPDATE concern SET id_product=@id_product, quantity=@quantity WHERE id_sellout=@id_sellout";
                            command1.Parameters.Clear();
                            command1.Parameters.AddWithValue("@id_sellout", entity.Id);
                            command1.Parameters.AddWithValue("@id_product", product.Id);
                            command1.Parameters.AddWithValue("@quantity", line.Quantity);
                            command1.ExecuteNonQuery();
                        } 
                        else
                        {
                            return false;
                        }
                    }
                    return true;
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
