namespace DevToys.PocoDB.Enums
{
    public enum Enums
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

    internal enum ExecType { Scalar = 0, NonQuery = 1 }

    public enum PropBinding { ByAttribute = 0, ByPropertyName = 1, ByPropertyNameOrAttribute = 2 }
}