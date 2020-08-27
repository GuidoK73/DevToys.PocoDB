using DevToys.PocoDB.Attributes;

namespace PocoDBConsoleAppTest.Data
{ 
	/// <summary>
	/// Same as Company, DBBulkInsert Attribute can also be used on Company
	/// </summary>
	[DBBulkInsert("dbo.Company")]
	public class BulkCompany
	{
		[DBParameter("Id")]
		public int Id { get; set; } = 0;

		[DBParameter("Name")]
		public string Name { get; set; } = string.Empty;

		[DBParameter("Adress")]
		public string Adress { get; set; } = string.Empty;

		[DBParameter("Country")]
		public string Country { get; set; } = string.Empty;

		[DBParameter("ZipCode")]
		public string ZipCode { get; set; } = string.Empty;

		[DBParameter("HouseNumber")]
		public string HouseNumber { get; set; } = string.Empty;

		[DBParameter("CompanyType")]
		public CompanyType CompanyType { get; set; } = CompanyType.BV;
	}

	[DBBulkInsert("dbo.Company")]
	public class BulkCompanyRandom
	{
		[DBRandomParameter("Id")]
		public int Id { get; set; } = 0;

		[DBRandomParameter("Name", RandomStringType = RandomStringType.FullName, Items = new object[] { 1, 2 } )]
		public string Name { get; set; } = string.Empty;

		[DBRandomParameter("Adress", RandomStringType = RandomStringType.Adress)]
		public string Adress { get; set; } = string.Empty;

		[DBRandomParameter("Country", RandomStringType = RandomStringType.Country)]
		public string Country { get; set; } = string.Empty;

		[DBRandomParameter("ZipCode", RandomStringType = RandomStringType.ZipCode)]
		public string ZipCode { get; set; } = string.Empty;

		[DBRandomParameter("HouseNumber", RandomStringType = RandomStringType.Number, Min = 10, Max = 200)]
		public string HouseNumber { get; set; } = string.Empty;

		[DBRandomParameter("CompanyType")]
		public CompanyType CompanyType { get; set; } = CompanyType.BV;

		[DBRandomParameter("EncryptedText", RandomStringType = RandomStringType.Text, Max = 10, Encrypt = true)]
		public string EncryptedText { get; set; }
	}
}