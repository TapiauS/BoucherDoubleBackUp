namespace Boucher_DoubleModel.Models
{
    /// <summary>
    /// Enum of all the possible <class>Users</class> role
    /// </summary>
    public enum Role
    {
        NORMAL,
        ADMIN,
        SUPERADMIN
    }
    /// <summary>
    /// Help to define the function and methodes associated with the <see cref="Role"/> enum
    /// </summary>
    public static class RoleExtensions
    {
        /// <summary>
        /// Compare a string to a Role enum value
        /// </summary>
        /// <param name="role">The reference role</param>
        /// <param name="input">The string to compare</param>
        /// <returns></returns>
        public static bool CompareToString(this Role role, string input)
        {
            return role.ToString().Equals(input.ToUpper(), StringComparison.OrdinalIgnoreCase);
        }
        /// <summary>
        /// Parse a string into a Role
        /// </summary>
        /// <param name="input">The role name we try to parse</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Throw if no corresponding role where found</exception>
        public static Role GetRoleFromString(string input)
        {
            foreach (Role role in Enum.GetValues(typeof(Role)))
            {
                if (role.CompareToString(input))
                    return role;
            }

            throw new ArgumentException("Invalid role string: " + input);
        }
    }
}
