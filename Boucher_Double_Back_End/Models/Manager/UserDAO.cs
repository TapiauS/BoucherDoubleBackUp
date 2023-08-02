using Boucher_Double_Back_End.Models.ServerModel;
using Boucher_DoubleModel.Models;
using Boucher_DoubleModel.Models.Entitys;
using MySqlConnector;
using Npgsql;

namespace Boucher_Double_Back_End.Models.Manager
{
    /// <summary>
    /// Implementation of the <see cref="IDAO{User}"/> interface that interact with this associated database entities
    /// </summary>
    public class UserDAO : IDAO<User>
    {

        public Store Store { get ; set ; }
        public User User { get ; set ; }

        public async Task<int> CreateAsync(User entity)
        {
            if (User.Role >= Role.ADMIN)
            {
                try
                {
                    MySqlConnection connexion = Connexion.getConnexion();
                    
                    using MySqlCommand command = new();
                    command.Connection = connexion;
                    if (entity.Id == 0)
                    {
                        command.CommandText = "INSERT INTO person(name,surname,mail,phone_number) VALUES (@name,@surname,@mail,@phone) RETURNING id";
                        command.Parameters.AddWithValue("@name", entity.Name);
                        command.Parameters.AddWithValue("@surname", entity.Surname);
                        command.Parameters.AddWithValue("@mail", entity.Mail);
                        command.Parameters.AddWithValue("@phone", entity.PhoneNumber);
                        using MySqlDataReader reader = command.ExecuteReader();
                        if (await reader.ReadAsync())
                        {
                            int id = reader.GetInt32(0);
                            MySqlConnection connexion1 = Connexion.getConnexion();

                            using MySqlCommand command1 = new();
                            command1.Connection = connexion1;
                            command1.CommandText = "INSERT INTO _user(pseudo,password,id_person,id_store,id_role) VALUES (@login,@password,@id_person,@id_store,(SELECT id FROM role WHERE name=@role)) RETURNING id";
                            command1.Parameters.Clear();
                            command1.Parameters.AddWithValue("@login", entity.Login);
                            command1.Parameters.AddWithValue("@password", Security.Hash(entity.Password));
                            command1.Parameters.AddWithValue("@id_person", id);
                            command1.Parameters.AddWithValue("@role", entity.Role.ToString().ToLower());
                            if (Store!=null)
                                command1.Parameters.AddWithValue("@id_store", entity.Store.IdStore);
                            else
                                command1.Parameters.AddWithValue("@id_store", DBNull.Value);
                            using MySqlDataReader reader1 = command1.ExecuteReader();
                            if (await reader1.ReadAsync())
                                return reader1.GetInt32(0);
                            else
                                return 0;
                        }
                        return 0;
                    }
                    else
                    {
                        command.CommandText = "INSERT INTO _user(login,password,id_person,id_store) VALUES (@login,@password,@id_person,@id_store) RETURNING id";
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@login", entity.Login);
                        command.Parameters.AddWithValue("@password", Security.Hash(entity.Password));
                        command.Parameters.AddWithValue("@id_person", entity.Id);
                        command.Parameters.AddWithValue("@id_store", entity.Store.Id);
                        using MySqlDataReader reader = command.ExecuteReader();
                        if (await reader.ReadAsync())
                            return reader.GetInt32(0);
                        else
                            return 0;
                    }
                }
                catch (IOException ioe)
                {
                    throw new DAOException("Erreur de connexion : "+ioe.Message, ioe, ErrorTypeDAO.IOE);
                }
                catch (MySqlException msqe)
                {
                    switch (msqe.ErrorCode)
                    {
                        case MySqlErrorCode.DuplicateKeyEntry:
                            return 0;
                            break;
                        default:
                            throw new DAOException("Erreur SQL grave : "+msqe.Message, msqe, ErrorTypeDAO.SQLSEVERE);
                    }
                }
                catch (Exception e)
                {
                    throw new DAOException("Erreur inconnue : "+e.Message, e, ErrorTypeDAO.UNKNOW);
                }

            }
            else
                return 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (User.Role >= Role.ADMIN)
            {
                try
                {
                    MySqlConnection connexion = Connexion.getConnexion();
                    
                    using MySqlCommand command = new();
                    command.Connection = connexion;
                    command.CommandText = "DELETE FROM _user WHERE id=@id";
                    command.Parameters.AddWithValue("@id", id);
                    return await command.ExecuteNonQueryAsync() > 0;
                }
                catch (IOException ioe)
                {
                    throw new DAOException("Erreur de connexion : "+ioe.Message, ioe, ErrorTypeDAO.IOE);
                }
                catch (MySqlException msqe)
                {
                    switch (msqe.ErrorCode)
                    {
                        case MySqlErrorCode.DuplicateKeyEntry:
                            return default;
                            break;
                        default:
                            throw new DAOException("Erreur SQL grave : "+msqe.Message, msqe, ErrorTypeDAO.SQLSEVERE);
                    }
                }
                catch (Exception e)
                {
                    throw new DAOException("Erreur inconnue : "+e.Message, e, ErrorTypeDAO.UNKNOW);
                }
            }
            else
                return false; 
        }

        public async Task<List<User>> GetAllAsync()
        {
            try
            {
                if (User.Role >= Role.ADMIN)
                {
                    MySqlConnection connexion = Connexion.getConnexion();
                    
                    using MySqlCommand command = new();
                    command.Connection = connexion;
                    command.CommandText = "SELECT * FROM full_user";
                    using MySqlDataReader reader = command.ExecuteReader();
                    List<User> users = new();
                    while (await reader.ReadAsync())
                    {
                        users.Add(new()
                        {
                            Surname = reader.GetString(reader.GetOrdinal("surname")),
                            Password="",
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Login = reader.GetString(reader.GetOrdinal("pseudo")),
                            Store = await new StoreDAO().GetByIdAsync(reader.GetInt32(reader.GetOrdinal("id_store"))),
                            IdUser = reader.GetInt32(reader.GetOrdinal("id_user")),
                            Mail = reader.GetString(reader.GetOrdinal("mail")),
                            Role = await new RoleDAO().GetByIdAsync(reader.GetInt32(reader.GetOrdinal("id_role")))
                        });
                    }
                    return users;
                }
                else
                    throw new Exception("Cant request if not an admin");
            }
            catch (IOException ioe)
            {
                throw new DAOException("Erreur de connexion : " + ioe.Message, ioe, ErrorTypeDAO.IOE);
            }
            catch (MySqlException msqe)
            {
                switch (msqe.ErrorCode)
                {
                    case MySqlErrorCode.DuplicateKeyEntry:
                        return default;
                        break;
                    default:
                        throw new DAOException("Erreur SQL grave : "+msqe.Message, msqe, ErrorTypeDAO.SQLSEVERE);
                }
            }
            catch (Exception e)
            {
                throw new DAOException("Erreur inconnue : "+e.Message, e, ErrorTypeDAO.UNKNOW);
            }
        }

        public async Task<User> GetByIdAsync(int id)
        {
            try
            {
                if (User.Role >= Role.ADMIN)
                {
                    MySqlConnection connexion = Connexion.getConnexion();
                    using MySqlCommand command = new();
                    command.Connection = connexion;
                    command.CommandText = "SELECT * FROM full_user WHERE id_user=@id";
                    command.Parameters.AddWithValue("@id", id);
                    using MySqlDataReader reader = command.ExecuteReader();
                    User users = new();
                    while (await reader.ReadAsync())
                    {
                        users = new()
                        {
                            Surname = reader.GetString(reader.GetOrdinal("surname")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Password="",
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Login = reader.GetString(reader.GetOrdinal("pseudo")),
                            Store = await new StoreDAO().GetByIdAsync(reader.GetInt32(reader.GetOrdinal("id_store"))),
                            IdUser = reader.GetInt32(reader.GetOrdinal("id_user")),
                            Mail = reader.GetString(reader.GetOrdinal("mail")),
                            Role =await new RoleDAO().GetByIdAsync(reader.GetInt32(reader.GetOrdinal("id_role")))
                        };
                    }
                    return users;
                }
                else
                    throw new Exception("Cant request if not an admin");
            }
            catch (IOException ioe)
            {
                throw new DAOException("Erreur de connexion : " + ioe.Message, ioe, ErrorTypeDAO.IOE);
            }
            catch (MySqlException msqe)
            {
                switch (msqe.ErrorCode)
                {
                    case MySqlErrorCode.DuplicateKeyEntry:
                        return default;
                        break;
                    default:
                        throw new DAOException("Erreur SQL grave : " +msqe.Message, msqe, ErrorTypeDAO.SQLSEVERE);
                }
            }
            catch (Exception e)
            {
                throw new DAOException("Erreur inconnue : "+e.Message, e, ErrorTypeDAO.UNKNOW);
            }
        }

        public async Task<bool> UpdateAsync(User entity)
        {
            try
            {
                if (User.Role >= Role.ADMIN)
                {
                    using MySqlConnection connection = Connexion.getConnexion();

                    using MySqlCommand command = new()
                    {
                        Connection = connection,
                        CommandText = "UPDATE _user SET pseudo=@login, id_person=@id_person, id_store=@id_store, password=@password WHERE id=@id"
                    };

                    command.Parameters.AddWithValue("@login", entity.Login);
                    command.Parameters.AddWithValue("@id_person", entity.Id);
                    command.Parameters.AddWithValue("@id_store", entity.Store.IdStore);
                    command.Parameters.AddWithValue("@password", Security.Hash(entity.Password));
                    command.Parameters.AddWithValue("@id", entity.IdUser);
                    return await command.ExecuteNonQueryAsync()> 0;
                }
                else
                {
                    throw new Exception("Cannot update user if not an admin");
                }
            }
            catch (IOException ioe)
            {
                throw new DAOException("Erreur de connexion : "+ioe.Message, ioe, ErrorTypeDAO.IOE);
            }
            catch (MySqlException msqe)
            {
                switch (msqe.ErrorCode)
                {
                    case MySqlErrorCode.DuplicateKeyEntry:
                        return default;
                        break;
                    default:
                        throw new DAOException("Erreur SQL grave : " + msqe.Message, msqe, ErrorTypeDAO.SQLSEVERE);
                }
            }
            catch (Exception e)
            {
                throw new DAOException("Erreur inconnue : "+e.Message, e, ErrorTypeDAO.UNKNOW);
            }
        }

        /// <summary>
        /// Connect an user using its login and password information
        /// </summary>
        /// <param name="username">The login of the user</param>
        /// <param name="password">The password of the user</param>
        /// <returns>The user if both information are correct, null elsewhere</returns>
        /// <exception cref="DAOException"></exception>
        public async Task<User?> Connect(string username, string password)
        {
            try
            {
                using MySqlConnection connexion = Connexion.getConnexion();
                using MySqlCommand command = new();
                command.Connection = connexion;
                command.CommandText = "SELECT * FROM full_user WHERE pseudo=@login";
                command.Parameters.AddWithValue("login", username);
                using MySqlDataReader reader = command.ExecuteReader();
                User user = null;
                if (await reader.ReadAsync() && Security.Validate(reader.GetString(reader.GetOrdinal("password")), password))
                {
                    if (reader.IsDBNull(reader.GetOrdinal("id_store")))
                    {
                        user = new User()
                        {
                            IdUser = reader.GetInt32(reader.GetOrdinal("id")),
                            Login = username,
                            Role = await new RoleDAO().GetByIdAsync(reader.GetInt32(reader.GetOrdinal("id_role")))
                        };
                        int storeId = 0;
                        Store store = await new StoreDAO().GetByIdAsync(storeId);
                        user.Store = store;
                        return user;
                    }
                    else
                    {
                        user = new User()
                        {
                            IdUser = reader.GetInt32(reader.GetOrdinal("id_user")),
                            Login = username,
                            Password = password,
                            Role = await new RoleDAO().GetByIdAsync(reader.GetInt32(reader.GetOrdinal("id_role"))),
                            Mail = reader.GetString(reader.GetOrdinal("mail")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Surname = reader.GetString(reader.GetOrdinal("surname")),
                            Id = reader.GetInt32(reader.GetOrdinal("id"))
                        };
                        int storeId = reader.GetInt32(reader.GetOrdinal("id_store"));
                        Store store = await new StoreDAO() { Store = Store,User=User }.GetByIdAsync(storeId);
                        user.Store = store;
                        //Update of the lat used date , used for rgpd suppression of account
                        MySqlConnection connectionnotif = Connexion.getConnexion();
                        MySqlCommand commandnotif = new () { Connection = connectionnotif,CommandText="UPDATE person SET last_used=NOW() WHERE id=@id" };
                        commandnotif.Parameters.AddWithValue("@id",user.Id);
                        commandnotif.ExecuteReader();
                        return user;
                    }
                }

                else
                    return null;

            }
            catch (IOException ioe)
            {
                throw new DAOException("Erreur de connexion : "+ioe.Message, ioe, ErrorTypeDAO.IOE);
            }
            catch (MySqlException msqe)
            {
                switch (msqe.ErrorCode)
                {
                    case MySqlErrorCode.DuplicateKeyEntry:
                        return null;
                        break;
                    default:
                        throw new DAOException("Erreur SQL grave : "+msqe.Message, msqe, ErrorTypeDAO.SQLSEVERE);
                }
            }
            catch (Exception e)
            {
                throw new DAOException("Erreur inconnue : "+e.Message, e, ErrorTypeDAO.UNKNOW);
            }

        }
    }
}
