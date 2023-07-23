using Boucher_DoubleModel.Models.Entitys;
using MySqlConnector;

namespace Boucher_Double_Back_End.Models.Manager
{
    /// <summary>
    /// Implementation of the <see cref="IDAO{MailParameter}"/> interface that interact with this associated database entities
    /// </summary>
    public class MailParameterDAO : IDAO<MailParameter>
    {
        public Store Store { get; set; }
        public User User { get ; set; }

        public async Task<int> CreateAsync(MailParameter entity)
        {
            try
            {
                using MySqlConnection connexion = Connexion.getConnexion();

                using MySqlCommand command = new();
                command.Connection = connexion;
                command.CommandText = "INSERT INTO mail_parameter( connection_type, server, password, port, id_store,login) VALUES ( @connexion_type, @server, @password, @port, @idstore,@login) RETURNING id";
                command.Parameters.AddWithValue("@connexion_type", entity.ConnexionType.ToString());
                command.Parameters.AddWithValue("@server", entity.Server);
                command.Parameters.AddWithValue("@password", entity.Password);
                command.Parameters.AddWithValue("@port", entity.Port);
                command.Parameters.AddWithValue("@idstore", Store.IdStore);
                command.Parameters.AddWithValue("@login",entity.Login==""?null:entity.Login);
                using MySqlDataReader reader = command.ExecuteReader();
                if (await reader.ReadAsync())
                    return reader.GetInt32(0);
                else
                    return 0;
            }
            catch (Exception ex)
            {
                throw new DAOException(ex.Message,ex);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                using MySqlConnection connexion = Connexion.getConnexion();

                using MySqlCommand command = new();
                command.Connection = connexion;
                command.CommandText = "DELETE FROM mail_parameter WHERE id = @id AND id_store = @idstore";
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@idstore", Store.IdStore);
                return await command.ExecuteNonQueryAsync() > 0;
            }
            catch (Exception ex)
            {
                throw new DAOException(ex.Message, ex);
            }

        }

        public async Task<List<MailParameter>> GetAllAsync()
        {
            try
            {
                using MySqlConnection connexion = Connexion.getConnexion();

                using MySqlCommand command = new MySqlCommand();
                command.Connection = connexion;
                command.CommandText = "SELECT * FROM mail_parameter WHERE id_store = @idstore";
                command.Parameters.AddWithValue("@idstore", Store.IdStore);
                using MySqlDataReader reader = command.ExecuteReader();
                List<MailParameter> mailParameters = new();
                while (await reader.ReadAsync())
                {
                    mailParameters.Add(new MailParameter()
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        ConnexionType = MailParameter.GetOption(reader.GetString(reader.GetOrdinal("connection_type"))),
                        Port = reader.GetInt32(reader.GetOrdinal("port")),
                        Password = reader.GetString(reader.GetOrdinal("password")),
                        Server = reader.GetString(reader.GetOrdinal("server")),
                        Login=reader.IsDBNull(reader.GetOrdinal("login"))?null:reader.GetString(reader.GetOrdinal("login"))
                    });
                }
                return mailParameters;
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

        public async Task<MailParameter> GetByIdAsync(int id)
        {
            try
            {
                using MySqlConnection connexion = Connexion.getConnexion();

                using MySqlCommand command = new ();
                command.Connection = connexion;
                command.CommandText = "SELECT * FROM mail_parameter WHERE id = @id AND id_store = @idstore";
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@idstore", Store.IdStore);
                using MySqlDataReader reader = command.ExecuteReader();
                MailParameter mailParameter = new();
                if (await reader.ReadAsync())
                {
                    mailParameter = new MailParameter()
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        ConnexionType = MailParameter.GetOption(reader.GetString(reader.GetOrdinal("connection_type"))),
                        Port = reader.GetInt32(reader.GetOrdinal("port")),
                        Password = reader.GetString(reader.GetOrdinal("password")),
                        Server = reader.GetString(reader.GetOrdinal("server")),
                        Login= reader.IsDBNull(reader.GetOrdinal("login")) ? null : reader.GetString(reader.GetOrdinal("login"))
                    };
                }
                return mailParameter;
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

        public async Task<bool> UpdateAsync(MailParameter entity)
        {
            try
            {
                using MySqlConnection connexion = Connexion.getConnexion();

                using MySqlCommand command = new MySqlCommand();
                command.Connection = connexion;
                command.CommandText = "UPDATE mail_parameter SET  connection_type = @connexion_type, server = @server, password = @password, port = @port,login=@login WHERE id = @id AND id_store = @idstore";
                command.Parameters.AddWithValue("@connexion_type", entity.ConnexionType.ToString()); ;
                command.Parameters.AddWithValue("@server", entity.Server);
                command.Parameters.AddWithValue("@password", entity.Password);
                command.Parameters.AddWithValue("@port", entity.Port);
                command.Parameters.AddWithValue("@id", entity.Id);
                command.Parameters.AddWithValue("@idstore", Store.IdStore);
                command.Parameters.AddWithValue("@login", entity.Login == "" ? null : entity.Login);
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
    }
}
