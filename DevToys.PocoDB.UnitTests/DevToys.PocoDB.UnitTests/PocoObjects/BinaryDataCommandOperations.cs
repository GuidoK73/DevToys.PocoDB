using DevToys.PocoDB.Attributes;
using System.Data;

namespace PocoDBConsoleAppTest.Data
{
    [DBCommand("Insert into dbo.BinaryData (Name, Photo) values (@Name, @Photo);", commandtype: CommandType.Text)]
    public class InsertPhoto
    {
        [DBParameter("Name")]
        public string Name { get; set; }

        [DBParameter("Photo")]
        public byte[] Photo { get; set; }
    }
}
