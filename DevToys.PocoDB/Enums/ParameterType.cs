namespace GK.Data.Enums
{
    /// <summary>
    /// </summary>
    public enum ParameterType
    {
        /// <summary>
        /// Property relates normally to a DBParameter  (Default)
        /// </summary>
        Normal = 0,

        /// <summary>
        /// Used for stored procedure parameters.
        /// </summary>
        Output = 1
    }
}