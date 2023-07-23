using Boucher_DoubleModel.Models.Entitys;
using MySqlConnector;

namespace Boucher_Double_Back_End.Models.Manager
{
    public class MailContentDAO : IDAO<MailContentParameter>
    {
        public Store  ?Store { get ; set ; }
        public User  ?User { get ; set ; }

        public async Task<int> CreateAsync(MailContentParameter entity)
        {
            try
            {
                using MySqlConnection mySqlConnection = Connexion.getConnexion();
                using MySqlCommand command = new MySqlCommand() { Connection = mySqlConnection };
                command.CommandText = "INSERT INTO mail_content_parameter(name,id_store,mail_head,object,mail_foot) VALUES (@name,@id_store,@mail_head,@object,@mail_foot) RETURNING id";
                command.Parameters.AddWithValue("@name", entity.Name);
                command.Parameters.AddWithValue("@id_store", Store.IdStore);
                command.Parameters.AddWithValue("@mail_head", entity.MailHead);
                command.Parameters.AddWithValue("@object", entity.MailObject);
                command.Parameters.AddWithValue("@mail_foot", entity.MailFoot);
                using MySqlDataReader reader = command.ExecuteReader();
                if (await reader.ReadAsync())
                    return reader.GetInt32(0);
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
                using MySqlConnection mySqlConnection = Connexion.getConnexion();
                using MySqlCommand command = new() { Connection = mySqlConnection };
                command.CommandText = "DELETE FROM mail_content_parameter WHERE id=@id AND id_store=@idstore";
                command.Parameters.AddWithValue("@idstore", Store.IdStore);
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

        public async Task<List<MailContentParameter>> GetAllAsync()
        {
            try
            {
                using MySqlConnection mySqlConnection = Connexion.getConnexion();
                using MySqlCommand command = new() { Connection = mySqlConnection };
                command.CommandText = "SELECT * FROM mail_content_parameter WHERE id_store=@idstore";
                command.Parameters.AddWithValue("@idstore", Store.IdStore);
                List<MailContentParameter> list = new();
                using MySqlDataReader reader = command.ExecuteReader();
                while (await reader.ReadAsync())
                {
                    list.Add(new()
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        Name = reader.GetString(reader.GetOrdinal("name")),
                        MailFoot = reader.GetString(reader.GetOrdinal("mail_foot")),
                        MailObject = reader.GetString(reader.GetOrdinal("object")),
                        MailHead = reader.GetString(reader.GetOrdinal("mail_head")),
                        IdStore = Store.IdStore,
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

        public async Task<MailContentParameter> GetByIdAsync(int id)
        {
            try
            {
                using MySqlConnection mySqlConnection = Connexion.getConnexion();
                using MySqlCommand command = new() { Connection = mySqlConnection };
                command.CommandText = "SELECT * FROM mail_content_parameter WHERE id_store=@idstore";
                command.Parameters.AddWithValue("@idstore", Store.IdStore);
                MailContentParameter list = null;
                using MySqlDataReader reader = command.ExecuteReader();
                while (await reader.ReadAsync())
                {
                    list = new()
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        Name = reader.GetString(reader.GetOrdinal("name")),
                        MailFoot = reader.GetString(reader.GetOrdinal("mail_foot")),
                        MailObject = reader.GetString(reader.GetOrdinal("object")),
                        MailHead = reader.GetString(reader.GetOrdinal("mail_head")),
                        IdStore = Store.IdStore,
                    };
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

        public async Task<bool> UpdateAsync(MailContentParameter entity)
        {
            try
            {
                using MySqlConnection mySqlConnection = Connexion.getConnexion();
                using MySqlCommand command = new MySqlCommand() { Connection = mySqlConnection };
                command.CommandText = "UPDATE mail_content_parameter SET name=@name,mail_head=@mail_head,object=@object,mail_foot=@mail_foot WHERE id=@id AND id_store=@id_store ";
                command.Parameters.AddWithValue("@name", entity.Name);
                command.Parameters.AddWithValue("@id_store", Store.IdStore);
                command.Parameters.AddWithValue("@id", entity.Id);
                command.Parameters.AddWithValue("@mail_head", entity.MailHead);
                command.Parameters.AddWithValue("@object", entity.MailObject);
                command.Parameters.AddWithValue("@mail_foot", entity.MailFoot);
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
    }
}
