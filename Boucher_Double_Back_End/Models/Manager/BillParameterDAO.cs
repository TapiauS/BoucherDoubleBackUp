using Boucher_DoubleModel.Models.Entitys;
using MySqlConnector;

namespace Boucher_Double_Back_End.Models.Manager
{
    /// <summary>
    /// Implementation of the <see cref="IDAO{BillParameter}"/> interface that interact with this associated database entities
    /// </summary>
    public class BillParameterDAO : IDAO<BillParameter>
    {
        public Store Store { get ; set; }
        public User User { get; set; }

        public async Task<int> CreateAsync(BillParameter entity)
        {
            try
            {
                using MySqlConnection connexion = Connexion.getConnexion();
                
                using MySqlCommand command = new();
                command.Connection = connexion;
                command.CommandText = "INSERT INTO bill_parameter(name,foot,special_mention,mention,id_store) VALUES (@name,@foot,@special_mention,@mention,@idstore) RETURNING id";
                command.Parameters.AddWithValue("@name", entity.Name);
                command.Parameters.AddWithValue("@foot", entity.Foot);
                command.Parameters.AddWithValue("@special_mention", entity.SpecialMention);
                command.Parameters.AddWithValue("@mention", entity.Mention);
                command.Parameters.AddWithValue("@idstore", Store.IdStore);
                using MySqlDataReader reader = command.ExecuteReader();
                if (await reader.ReadAsync())
                {
                    return reader.GetInt32(reader.GetOrdinal("id"));
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
                
                using MySqlCommand command = new ();
                command.Connection = connexion;
                command.CommandText = "DELETE FROM bill_parameter WHERE id=@id AND id_store=@idstore";
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
    

        public async Task<List<BillParameter>> GetAllAsync()
        {
            try
            {
                using MySqlConnection connexion = Connexion.getConnexion();
                
                using MySqlCommand command = new();
                command.Connection = connexion;
                command.CommandText = "SELECT * FROM bill_parameter WHERE id_store=@idstore";
                command.Parameters.AddWithValue("@idstore", Store.IdStore);
                using MySqlDataReader reader = command.ExecuteReader();
                List<BillParameter> returns = new();
                while (await reader.ReadAsync())
                {
                    returns.Add(new BillParameter()
                    {
                        Name = reader.GetString(reader.GetOrdinal("name")),
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        Foot = reader.GetString(reader.GetOrdinal("foot")),
                        SpecialMention = reader.GetString(reader.GetOrdinal("special_mention")),
                        Mention = reader.GetString(reader.GetOrdinal("mention")),
                        IdStore = Store.Id,
                    });
                }
                return returns;
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

        public async Task<BillParameter> GetByIdAsync(int id)
        {
            try
            {
                using MySqlConnection connexion = Connexion.getConnexion();
                
                using MySqlCommand command = new();
                command.Connection = connexion;
                command.CommandText = "SELECT * FROM bill_parameter WHERE id=@id AND id_store=@idstore";
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@idstore", Store.IdStore);
                using MySqlDataReader reader = command.ExecuteReader();
                BillParameter returns = new();
                if (await reader.ReadAsync())
                {
                    returns = new BillParameter()
                    {
                        Name = reader.GetString(reader.GetOrdinal("name")),
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        Foot = reader.GetString(reader.GetOrdinal("foot")),
                        SpecialMention = reader.GetString(reader.GetOrdinal("special_mention")),
                        Mention = reader.GetString(reader.GetOrdinal("mention")),
                        IdStore=Store.Id
                    };
                }
                return returns;
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

        public async Task<bool> UpdateAsync(BillParameter entity)
        {
            try
            {
                using MySqlConnection connexion = Connexion.getConnexion();
                
                using MySqlCommand command = new();
                command.Connection = connexion;
                command.CommandText = "UPDATE bill_parameter SET name=@name,foot=@foot,special_mention=@special_mention,mention=@mention WHERE id=@id AND id_store=@idstore";
                command.Parameters.AddWithValue("@name", entity.Name);
                command.Parameters.AddWithValue("@foot", entity.Foot);
                command.Parameters.AddWithValue("@special_mention", entity.SpecialMention);
                command.Parameters.AddWithValue("@mention", entity.Mention);
                command.Parameters.AddWithValue("@id", entity.Id);
                command.Parameters.AddWithValue("@idstore", Store.IdStore);
                return await command.ExecuteNonQueryAsync()>0;
            }
            catch (IOException ioe)
            {
                throw new DAOException("Erreur de connexion",ioe,ErrorTypeDAO.IOE);
            }
            catch(MySqlException msqe)
            {
                switch(msqe.ErrorCode)
                {
                    case MySqlErrorCode.DuplicateKeyEntry:
                        return false;
                        break;
                    default:
                        throw new DAOException("Erreur SQL grave", msqe, ErrorTypeDAO.SQLSEVERE);
                }
            }
            catch(Exception e)
            {
                throw new DAOException("Erreur inconnue", e, ErrorTypeDAO.UNKNOW);
            }
        }
    }
}
