using Boucher_DoubleModel.Models.Entitys;
using MySqlConnector;
using System.Runtime.CompilerServices;

namespace Boucher_Double_Back_End.Models.Manager
{
    public class ClientDAO : IDAO<Client>
    {
        public Store Store { get; set; }
        public User User { get; set ; }

        public async Task<int> CreateAsync(Client entity)
        {
            try
            {
                using MySqlConnection checkAvailableConnexion=Connexion.getConnexion();
                using MySqlCommand checkAvailableCommand = checkAvailableConnexion.CreateCommand();
                checkAvailableCommand.CommandText = "SELECT id FROM person WHERE mail=@mail OR phone_number=@phone_number";
                checkAvailableCommand.Parameters.AddWithValue("@mail", entity.Mail);
                checkAvailableCommand.Parameters.AddWithValue("@phone_number", entity.PhoneNumber);
                using MySqlDataReader checkReader=checkAvailableCommand.ExecuteReader();
                if(await checkReader.ReadAsync()) 
                {
                    int idclient = checkReader.GetInt32(0);
                    using MySqlConnection connexion = Connexion.getConnexion();
                    using MySqlCommand command = connexion.CreateCommand();
                    command.Connection = connexion;
                    command.CommandText = "INSERT INTO buy(id_store, id_person) VALUES (@idstore, @idperson)";
                    command.Parameters.AddWithValue("@idstore", Store.IdStore);
                    command.Parameters.AddWithValue("@idperson", idclient);
                    if (await command.ExecuteNonQueryAsync() > 0)
                        return idclient;
                    else
                        return 0;
                }
                else
                {
                    using MySqlConnection connexion = Connexion.getConnexion();
                    using MySqlCommand command = connexion.CreateCommand();
                    command.CommandText = "INSERT INTO person(name, surname, mail,phone_number) VALUES (@name, @surname, @mail,@phone) RETURNING id";
                    command.Parameters.AddWithValue("@name", entity.Name);
                    command.Parameters.AddWithValue("@surname", entity.Surname);
                    command.Parameters.AddWithValue("@mail", entity.Mail);
                    command.Parameters.AddWithValue("@phone", entity.PhoneNumber);
                    using MySqlDataReader reader = command.ExecuteReader();
                    if (await reader.ReadAsync())
                    {
                        int idclient = reader.GetInt32(0);
                        using MySqlConnection connexion1 = Connexion.getConnexion();
                        using MySqlCommand command1 = connexion1.CreateCommand();
                        command1.CommandText = "INSERT INTO buy(id_store, id_person) VALUES (@idstore, @idperson)";
                        command1.Parameters.AddWithValue("@idstore", Store.IdStore);
                        command1.Parameters.AddWithValue("@idperson", idclient);
                        if (await command1.ExecuteNonQueryAsync() > 0)
                            return idclient;
                        else
                            return 0;
                    }
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
                using MySqlConnection connexion = Connexion.getConnexion();

                using MySqlCommand command = connexion.CreateCommand();
                command.Connection = connexion;
                command.CommandText = "DELETE FROM buy WHERE id_person = @idperson AND id_store = @idstore";
                command.Parameters.AddWithValue("@idperson", id);
                command.Parameters.AddWithValue("@idstore", Store.IdStore);
                if (await command.ExecuteNonQueryAsync() > 0)
                {
                    command.CommandText = "DELETE FROM person WHERE id = @idperson";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@idperson", id);
                    return await command.ExecuteNonQueryAsync() > 0;
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

        public async Task<List<Client>> GetAllAsync()
        {
            try
            {
                using MySqlConnection connexion = Connexion.getConnexion();

                using MySqlCommand command = connexion.CreateCommand();
                command.Connection = connexion;
                command.CommandText = "SELECT * FROM client WHERE id_store = @idstore";
                command.Parameters.AddWithValue("@idstore", Store.IdStore);
                using MySqlDataReader reader = command.ExecuteReader();
                List<Client> list = new List<Client>();
                while (await reader.ReadAsync())
                {
                    list.Add(new Client()
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        Name = reader.GetString(reader.GetOrdinal("name")),
                        Surname = reader.GetString(reader.GetOrdinal("surname")),
                        Mail = reader.GetString(reader.GetOrdinal("mail")),
                        PhoneNumber=reader.GetString(reader.GetOrdinal("phone_number"))
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

        public async Task<Client> GetByIdAsync(int id)
        {
            try
            {
                using MySqlConnection connexion = Connexion.getConnexion();

                using MySqlCommand command = connexion.CreateCommand();
                command.Connection = connexion;
                command.CommandText = "SELECT * FROM client WHERE id_store = @idstore AND id = @id";
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@idstore", Store.IdStore);
                using MySqlDataReader reader = command.ExecuteReader();
                Client client = new Client();
                if (await reader.ReadAsync())
                {
                    client = new Client()
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        Name = reader.GetString(reader.GetOrdinal("name")),
                        Surname = reader.GetString(reader.GetOrdinal("surname")),
                        Mail = reader.GetString(reader.GetOrdinal("mail")),
                        PhoneNumber = reader.GetString(reader.GetOrdinal("phone_number"))
                    };
                }
                return client;
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

        public async Task<bool> UpdateAsync(Client entity)
        {
            try
            {
                using MySqlConnection connexion = Connexion.getConnexion();

                using MySqlCommand command = connexion.CreateCommand();
                command.Connection = connexion;
                command.CommandText = "SELECT * FROM buy WHERE id_store = @idstore AND id_person = @idperson";
                command.Parameters.AddWithValue("@idperson", entity.Id);
                command.Parameters.AddWithValue("@idstore", Store.IdStore);
                using MySqlDataReader reader= command.ExecuteReader();
                if (await reader.ReadAsync())
                {
                    using MySqlConnection connexion1 = Connexion.getConnexion();

                    using MySqlCommand command1 = connexion.CreateCommand();
                    command1.Connection=connexion1;
                    command1.CommandText = "UPDATE person SET name = @name, surname = @surname, mail = @mail,phone_number=@phone WHERE id = @id";
                    command1.Parameters.AddWithValue("@name", entity.Name);
                    command1.Parameters.AddWithValue("@surname", entity.Surname);
                    command1.Parameters.AddWithValue("@mail", entity.Mail);
                    command1.Parameters.AddWithValue("@id", entity.Id);
                    command1.Parameters.AddWithValue("@phone", entity.PhoneNumber);
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
