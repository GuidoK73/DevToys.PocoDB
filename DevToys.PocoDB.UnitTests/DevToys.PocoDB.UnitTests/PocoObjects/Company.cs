using DevToys.PocoDB.Attributes;

namespace PocoDBConsoleAppTest.Data
{ 
	public enum CompanyType
    {
		BV = 1,
		NV = 2,
		LLC = 3,
		GMBH = 4
    }

	public class Company
	{
		[DBField("Id")]
		public int Id { get; set; }

		[DBField("Name")]
		public string Name { get; set; }

		[DBField("Adress")]
		public string Adress { get; set; } 

		[DBField("Country")]
		public string Country { get; set; }

		[DBField("ZipCode")]
		public string ZipCode { get; set; }

		[DBField("HouseNumber")]
		public string HouseNumber { get; set; }

		[DBField("CompanyType")]
		public CompanyType CompanyType { get; set; }

		[DBField("EncryptedText", Decrypt = true)]
		public string EncryptedText { get; set; }
	}
}
