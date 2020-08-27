using DevToys.PocoDB.Attributes;

namespace PocoDBConsoleAppTest.Data
{  
	public class BinaryData
	{
		[DBField("Id")]
		public int Id { get; set; }

		[DBField("Name")]
		public string Name { get; set; }

		[DBField("Photo")]
		public byte[] Photo { get; set; }

		[DBField("Document")]
		public byte[] Document { get; set; }
	}
}
