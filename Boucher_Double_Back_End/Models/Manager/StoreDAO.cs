using Boucher_DoubleModel.Models.Entitys;
using MySqlConnector;

namespace Boucher_Double_Back_End.Models.Manager
{
    /// <summary>
    /// Implementation of the <see cref="IDAO{Store}"/> interface that interact with this associated database entities 
    /// </summary>
    public class StoreDAO : IDAO<Store>
    {
        public Store Store { get ; set; }
        public User User { get ; set ; }

        public async Task<int> CreateAsync(Store entity)
        {
            try
            {
                using MySqlConnection connection = Connexion.getConnexion();

                if (entity.Id == 0)
                {
                    using MySqlCommand command = new()
                    {
                        Connection = connection,
                        CommandText = "INSERT INTO person(name, mail,phone_number) VALUES (@name, @mail,@phone) RETURNING id"
                    };
                    command.Parameters.AddWithValue("@name", entity.Name);
                    command.Parameters.AddWithValue("@mail", entity.Mail);
                    command.Parameters.AddWithValue("@phone", entity.PhoneNumber);
                    using MySqlDataReader reader = command.ExecuteReader();
                    if (await reader.ReadAsync())
                    {
                        using MySqlConnection connection1 = Connexion.getConnexion();
                        using MySqlCommand command1 = new()
                        {
                            Connection = connection1
                        };
                        int id = reader.GetInt32(0);
                        entity.Id = id;
                        command1.CommandText = "INSERT INTO store(town, adress, id_person, logo_path) VALUES (@town, @adress, @id_person, @logo_path) RETURNING id";
                        command1.Parameters.Clear();
                        command1.Parameters.AddWithValue("@town", entity.Town);
                        command1.Parameters.AddWithValue("@adress", entity.Adress);
                        command1.Parameters.AddWithValue("@id_person", id);
                        command1.Parameters.AddWithValue("@logo_path", entity.LogoPath);
                        using MySqlDataReader reader1 = command1.ExecuteReader();
                        if (await reader1.ReadAsync())
                            return reader1.GetInt32(0);
                        else
                            return 0;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    using MySqlCommand command = new()
                    {
                        Connection = connection,
                        CommandText = "INSERT INTO store(town, adress, id_person, logo_path) VALUES (@town, @adress, @id_person, @logo_path) RETURNING id"
                    };
                    command.Parameters.AddWithValue("@town", entity.Town);
                    command.Parameters.AddWithValue("@adress", entity.Adress);
                    command.Parameters.AddWithValue("@id_person", entity.Id);
                    command.Parameters.AddWithValue("@logo_path", entity.LogoPath);
                    using MySqlDataReader reader = command.ExecuteReader();
                    if (await reader.ReadAsync())
                        return reader.GetInt32(0);
                    else
                        return 0;
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

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                using MySqlConnection connection = Connexion.getConnexion();

                using MySqlCommand command = new()
                {
                    Connection = connection,
                    CommandText = "DELETE FROM person WHERE id = @id"
                };
                command.Parameters.AddWithValue("@id", id);
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

        public async Task<List<Store>> GetAllAsync()
        {
            try
            {
                using MySqlConnection connection = Connexion.getConnexion();

                using MySqlCommand command = new()
                {
                    Connection = connection,
                    CommandText = "SELECT * FROM full_store"
                };

                using MySqlDataReader reader = command.ExecuteReader();
                List<Store> list = new();

                while (await reader.ReadAsync())
                {
                    Store store = new ()
                    {
                        Adress = reader.GetString(reader.GetOrdinal("adress")),
                        Town = reader.GetString(reader.GetOrdinal("town")),
                        Name = reader.GetString(reader.GetOrdinal("name")),
                        LogoPath = reader.GetString(reader.GetOrdinal("logo_path")),
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        IdStore = reader.GetInt32(reader.GetOrdinal("id_store")),
                        PhoneNumber = reader.GetString(reader.GetOrdinal("phone_number"))
                    };

                    list.Add(store);
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

        public async Task<Store> GetByIdAsync(int id)
        {
            try
            {
                using MySqlConnection connection = Connexion.getConnexion();

                using MySqlCommand command = new()
                {
                    Connection = connection,
                    CommandText = "SELECT * FROM full_store WHERE id_store=@id"
                };

                command.Parameters.AddWithValue("id", id);

                using MySqlDataReader reader = command.ExecuteReader();
                Store store = null;

                if (await reader.ReadAsync())
                {
                    store = new Store()
                    {
                        Adress = reader.GetString(reader.GetOrdinal("adress")),
                        Town = reader.GetString(reader.GetOrdinal("town")),
                        Name = reader.GetString(reader.GetOrdinal("name")),
                        LogoPath = reader.GetString(reader.GetOrdinal("logo_path")),
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        Mail = reader.GetString(reader.GetOrdinal("mail")),
                        IdStore = reader.GetInt32(reader.GetOrdinal("id_store")),
                        PhoneNumber = reader.GetString(reader.GetOrdinal("phone_number"))
                    };
                }

                return store;
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

        public async Task<bool> UpdateAsync(Store entity)
        {
            try
            {
                using MySqlConnection connection = Connexion.getConnexion();

                using MySqlCommand command = new()
                {
                    Connection = connection,
                    CommandText = "UPDATE person SET name=@name,mail=@mail WHERE id=@id"
                };
                command.Parameters.AddWithValue("name", entity.Name);
                command.Parameters.AddWithValue("mail", entity.Mail);
                command.Parameters.AddWithValue("id", entity.Id);
                if (await command.ExecuteNonQueryAsync() > 0)
                {
                    using MySqlConnection connection1 = Connexion.getConnexion();

                    using MySqlCommand command1 = new();
                    command1.Connection = connection1;
                    command1.CommandText = "UPDATE store SET town=@town,adress=@adress,logo_path=@logo_path WHERE id=@id_store";
                    command1.Parameters.AddWithValue("town", entity.Town);
                    command1.Parameters.AddWithValue("adress", entity.Adress);
                    command1.Parameters.AddWithValue("id_store", entity.IdStore);
                    command1.Parameters.AddWithValue("logo_path", entity.LogoPath);
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
