namespace DevToys.PocoDB.Enums
{
    internal enum ExecType
    {
        Scalar = 0,
        NonQuery = 1
    }

    public enum StrictMapping
    {
        /// <summary>Fieldmapping determined by Config setting StrictMapping.</summary>
        ByConfigSetting = 0,
        /// <summary>Field is mapped strict, if PropertyType does not match with SqlReader field type an error will occur.</summary>
        True = 1,
        /// <summary>Field will be mapped to the property if possible (if not an error will occur). </summary>
        False = 2,
    }

    public enum ReaderDefaults
    {
        UseDefaultValueProperty = 0,
        /// <summary>DateTime</summary>
        DateTimeNow = 3,
        /// <summary>DateTime</summary>
        DateTimeToday = 4,
        /// <summary>TimeSpan</summary>
        TimeNow = 5,
        /// <summary>TimeSpan: cast null to Now time.</summary>
        DateTimeUTCNow = 6,
        /// <summary>Int, Double, Float, DateTime etc</summary>
        MinValue = 7,
        /// <summary>Int, Double, Float, DateTime etc</summary>
        MaxValue = 8,
        /// <summary> Guid: Cast null to Empty Guid</summary>
        GuidEmpty = 9,
        /// <summary>Guid: Cast null to new Guid</summary>
        GuidNew = 10,
        /// <summary> String: Cast null to empty string</summary>
        StringEmpty = 11
    }

    public enum RandomStringType
    {
        /// <summary>Random Name out of 500 first names</summary>
        FirstName = 0,
        /// <summary>Random Name out of 500 Last names</summary>
        LastName = 1,
        /// <summary>First name + Last name</summary>
        FullName = 2,
        /// <summary>Random word out of 500 words</summary>
        Word = 3,
        /// <summary>A text of random words (number of words limited by Max property)</summary>
        Text = 4,
        /// <summary>Random Company name out of 500 company namrs</summary>
        CompanyName = 5,
        /// <summary>Company name formatted as an URL</summary>
        Url = 6,
        /// <summary>Random Number between Min and Max</summary>
        Number = 7,
        /// <summary>Random Guid</summary>
        Guid = 8,
        /// <summary>@U = Uppercase char @L Lowercase char @N numeric char</summary>
        Format = 9,
        /// <summary></summary>
        Password = 10,
        /// <summary>Random Date between DateMin and DateMax</summary>
        DateTime = 11,
        /// <summary>Random Date between DateMin and DateMax, formatted with Format property</summary>
        DateTimeFormatted = 12,
        /// <summary>Random Country name out of 500 names</summary>
        Country = 13,
        /// <summary>Random dutch street name out of 500</summary>
        Street = 14,
        /// <summary>Country,  City, street, housenumber, ZipCode</summary>
        Adress = 15,
        /// <summary>Random generated zipcode</summary>
        ZipCode = 16,
        /// <summary>Random Color name </summary>
        ColorName = 17,
        /// <summary>Random 11 proof BSN number.</summary>
        BSNNumber = 18,
        /// <summary>Random item out of item array  </summary>
        Item = 19
    }

}