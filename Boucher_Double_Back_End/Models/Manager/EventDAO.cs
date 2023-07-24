using Boucher_DoubleModel.Models.Entitys;
using MySqlConnector;

namespace Boucher_Double_Back_End.Models.Manager
{
    public class EventDAO : IDAO<Event>
    {
        public Store Store { get ; set ; }
        public User User { get ; set ; }

        public async Task<int> CreateAsync(Event entity)
        {
            try
            {
                using MySqlConnection connection = Connexion.getConnexion();
                using MySqlCommand command = new() { Connection = connection };
                command.CommandText = "INSERT INTO event(name,start,end,id_store) VALUES (@name,@start,@end,@idstore) RETURNING id";
                command.Parameters.AddWithValue("@name", entity.Name);
                command.Parameters.AddWithValue("@start", entity.Begin);
                command.Parameters.AddWithValue("@end", entity.End);
                command.Parameters.AddWithValue("@idstore", Store.IdStore);
                using MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
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
                using MySqlCommand command = new() { Connection = connection };
                command.CommandText = "DELETE FROM event WHERE id=@id AND id_store=@id_store";
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

                    default:
                        throw new DAOException("Erreur SQL grave", msqe, ErrorTypeDAO.SQLSEVERE);
                }
            }
            catch (Exception e)
            {
                throw new DAOException("Erreur inconnue", e, ErrorTypeDAO.UNKNOW);
            }
        }

        public async Task<List<Event>> GetAllAsync()
        {
            try
            {
                using MySqlConnection connection = Connexion.getConnexion();
                using MySqlCommand command = new() { Connection = connection };
                command.CommandText = "SELECT * FROM event WHERE id_store=@idstore";
                command.Parameters.AddWithValue("@idstore",Store.IdStore);
                List<Event> list = new ();   
                using MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new() { Id = reader.GetInt32(reader.GetOrdinal("id")), Name = reader.GetString(reader.GetOrdinal("name")), Store = Store, Begin = reader.GetDateTime(reader.GetOrdinal("start")), End = reader.GetDateTime(reader.GetOrdinal("end")) });
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

                    default:
                        throw new DAOException("Erreur SQL grave", msqe, ErrorTypeDAO.SQLSEVERE);
                }
            }
            catch (Exception e)
            {
                throw new DAOException("Erreur inconnue", e, ErrorTypeDAO.UNKNOW);
            }
        }

        public async Task<Event> GetByIdAsync(int id)
        {
            try
            {
                using MySqlConnection connection = Connexion.getConnexion();
                using MySqlCommand command = new() { Connection = connection };
                command.CommandText = "SELECT * FROM event WHERE id_store=@idstore AND id=@id";
                command.Parameters.AddWithValue("@idstore", Store.IdStore);
                command.Parameters.AddWithValue("@id", id);
                Event eventR = new();
                using MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    eventR=new() { Id = reader.GetInt32(reader.GetOrdinal("id")), Name = reader.GetString(reader.GetOrdinal("name")), Store = Store, Begin = reader.GetDateTime(reader.GetOrdinal("start")), End = reader.GetDateTime(reader.GetOrdinal("end")) };
                }
                return eventR;
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

        public async Task<bool> UpdateAsync(Event entity)
        {
            try
            {
                using MySqlConnection connection = Connexion.getConnexion();
                using MySqlCommand command = new() { Connection = connection };
                command.CommandText = "UPDATE event SET name=@name,start=@start,end=@end WHERE id=@id AND id_store=@idstore";
                command.Parameters.AddWithValue("@name", entity.Name);
                command.Parameters.AddWithValue("@start", entity.Begin);
                command.Parameters.AddWithValue("@end", entity.End);
                command.Parameters.AddWithValue("@idstore", Store.IdStore);
                command.Parameters.AddWithValue("@id", entity.Id);
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
