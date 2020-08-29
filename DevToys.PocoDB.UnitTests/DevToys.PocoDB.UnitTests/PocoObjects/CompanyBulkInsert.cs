using DevToys.PocoDB.Attributes;
using DevToys.PocoDB.Enums;

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

		[DBRandomParameter("Name", RandomStringType = RandomStringType.FullName)]
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

		[DBParameter("Text")]
		public string Text { get; set; } = string.Empty; // Will drain performance.
	}
}