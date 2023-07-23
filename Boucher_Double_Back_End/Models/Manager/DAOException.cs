namespace Boucher_Double_Back_End.Models.Manager
{
    /// <summary>
    /// The exception associated with the IDAO interface implementation
    /// </summary>
    public class DAOException:Exception
    {
        public DAOException() : base() { }
        public DAOException(String message) : base(message)
        {

        }

        public DAOException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        public DAOException(ErrorTypeDAO errorTypeDAO) : base() 
        {
            TypeDAO= errorTypeDAO;
        }
        public DAOException(String message, ErrorTypeDAO errorTypeDAO) : base(message)
        {
            TypeDAO = errorTypeDAO;
        }

        public DAOException(string? message, Exception? innerException, ErrorTypeDAO errorTypeDAO) : base(message, innerException)
        {
            TypeDAO = errorTypeDAO;
        }

        public ErrorTypeDAO TypeDAO { get; set; }
    }
}
