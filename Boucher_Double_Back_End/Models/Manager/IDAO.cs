using Boucher_DoubleModel.Models.Entitys;
using MySqlConnector;

namespace Boucher_Double_Back_End.Models.Manager
{
    /// <summary>
    /// Interface representing the access to the database, the generic parameter determine which data to retrieve in the concrete implementations of its methods
    /// </summary>
    /// <typeparam name="T">The entity type we want to interact inside the database </typeparam>
    public interface IDAO<T>
    {
        /// <summary>
        /// Attribute that help to securise the application, egal to the stored value in the session and limit the interaction to the user associated shop 
        /// </summary>
        public Store Store { get; set; }
        /// <summary>
        /// Attribute that help to securise the application, egal to the stored value in the session and limit certain action to certain <see cref="User"/> <see cref="Role"/>
        /// </summary>
        public User User { get;  set; }
        /// <summary>
        /// Get one element form the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The instance of <see cref="T"/></returns>
        Task<T> GetByIdAsync(int id);
        /// <summary>
        /// Get all the accessible element from the database
        /// </summary>
        /// <returns>A list of the instance of <see cref="T"/></returns>
        Task<List<T>> GetAllAsync();
        /// <summary>
        /// Delete on element from the database using its id
        /// </summary>
        /// <param name="id">The id of the entity to delete</param>
        /// <returns>Return if the operation was a success or not</returns>
        Task<bool> DeleteAsync(int id);
        /// <summary>
        /// Update the database if the element is accessible to the user
        /// </summary>
        /// <param name="entity">The <see cref="T"/> entity that contains the updated values</param>
        Task<bool> UpdateAsync(T entity);
        /// <summary>
        /// Create an element in the database
        /// </summary>
        /// <param name="entity">The <see cref="T"/> entity that contains the inserted values</param>
        /// <returns>The success or failure of the operation</returns>
        Task<int> CreateAsync(T entity);

    }
}
