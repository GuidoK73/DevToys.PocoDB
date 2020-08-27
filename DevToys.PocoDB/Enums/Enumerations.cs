namespace DevToys.PocoDB.Enums
{ 
    internal enum ExecType { Scalar = 0, NonQuery = 1 }

    public enum StrictMapping
    {
        /// <summary>
        /// Fieldmapping determined by Config setting StrictMapping.
        /// </summary>
        ByConfigSetting = 0,
        /// <summary>
        /// Field is mapped strict, if PropertyType does not match with SqlReader field type an error will occur.
        /// </summary>
        True = 1,
        /// <summary>
        /// Field will be mapped to the property if possible (if not an error will occur).
        /// </summary>
        False = 2,
    }
}