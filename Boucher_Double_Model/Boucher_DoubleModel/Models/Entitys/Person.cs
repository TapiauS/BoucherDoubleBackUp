using System.Text.RegularExpressions;

namespace Boucher_DoubleModel.Models.Entitys
{
    /// <summary>
    /// Abstract representation of a person, human or moral
    /// </summary>
    public abstract class Person
    {
        private int id;
        private string name="";
        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value.Trim();
            }
        }
        private string mail = "";
        /// <summary>
        /// Mail of the person, can't be null
        /// </summary>
        public string Mail
        {
            get
            {
                return mail;
            }
            set
            {
                 mail = value.Trim();
            }
        }

        public string PhoneNumber { get; set; } = "";

    }
}
