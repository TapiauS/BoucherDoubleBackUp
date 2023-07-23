using Boucher_DoubleModel.Models.Entitys;
using MySqlConnector;

namespace Boucher_Double_Back_End.Models.Manager
{
    /// <summary>
    /// Implementation of the <see cref="IDAO{Role}"/> interface, due to the special nature of the role element only the <see cref="GetByIdAsync(int)"/> and <see cref="GetAll"/> 
    /// function are actually implemented
    /// </summary>
    public class RoleDAO : IDAO<Boucher_DoubleModel.Models.Role>
    {
        public Store Store { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public User User { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public async Task<int> CreateAsync(Boucher_DoubleModel.Models.Role entity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Boucher_DoubleModel.Models.Role>> GetAllAsync()
        {
            try
            {
                using MySqlConnection connexion = Connexion.getConnexion();
                
                using MySqlCommand command = new();
                command.Connection = connexion;
                command.CommandText = "SELECT * FROM ROLE";
                using MySqlDataReader reader = command.ExecuteReader();
                List<Boucher_DoubleModel.Models.Role> roles = new();
                while (await reader.ReadAsync())
                {
                    roles.Add(Boucher_DoubleModel.Models.RoleExtensions.GetRoleFromString(reader.GetString(reader.GetOrdinal("name"))));
                }
                return roles;
            }
            catch (Exception ex)
            {
                throw new DAOException(ex.Message, ex);
            }
        }

        public async Task<Boucher_DoubleModel.Models.Role> GetByIdAsync(int id)
        {
            try
            {
                using MySqlConnection connexion = Connexion.getConnexion();
                
                using MySqlCommand command = new();
                command.Connection = connexion;
                command.CommandText = "SELECT * FROM ROLE WHERE id=@id";
                command.Parameters.AddWithValue("id", id);
                using MySqlDataReader reader = command.ExecuteReader();
                Boucher_DoubleModel.Models.Role role = new();
                if (await reader.ReadAsync())
                {
                    role = Boucher_DoubleModel.Models.RoleExtensions.GetRoleFromString(reader.GetString(reader.GetOrdinal("name")));
                }
                return role;
            }
            catch (Exception ex)
            {
                throw new DAOException(ex.Message, ex);
            }
        }

        public async Task<bool> UpdateAsync(Boucher_DoubleModel.Models.Role entity)
        {
            throw new NotImplementedException();    
        }
    }
}
