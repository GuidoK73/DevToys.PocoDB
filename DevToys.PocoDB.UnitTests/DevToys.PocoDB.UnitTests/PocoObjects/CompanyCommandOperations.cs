using DevToys.PocoDB.Attributes;
using System.Collections.Generic;
using System.Data;

namespace PocoDBConsoleAppTest.Data
{ 

    [DBCommand(@"select id, [name], Adress, Country, ZipCode, HouseNumber, CompanyType, EncryptedText from dbo.Company where id = @Id", commandtype: CommandType.Text)]
    public class GetCompanyById_Sql
    {
        [DBParameter("Id")]
        public int Id{ get; set; }
    }


    [DBCommand(@"select id, [name], Adress, Country, ZipCode, HouseNumber, CompanyType, EncryptedText from dbo.Company", commandtype: CommandType.Text)]
    public class GetCompanyAll
    { }

    [DBCommand(@"dbo.GetCompany", commandtype: CommandType.StoredProcedure)]
    public class GetCompanyById
    {
        [DBParameter("Id")]
        public int Id { get; set; }
    }


    [DBCommand(@"dbo.GetCompanies", commandtype: CommandType.StoredProcedure)]
    public class GetCompanies
    {
        [DBStringArrayParameter("Ids")]
        public List<int> Id { get; set; }
    }

    // Same as GetCompanies
    [DBCommand(@"select * from dbo.Company where id in (select convert(int, [value]) from STRING_SPLIT (@ids, ','));", commandtype: CommandType.Text)]
    public class GetCompanies2
    {
        [DBStringArrayParameter("Ids")]
        public int[] Id { get; set; }
    }

    // Same as GetCompanies
    [DBCommand(@"Delete from dbo.Company", commandtype: CommandType.Text)]
    public class DeleteAllCompanies
    { }


    [DBCommand(@"dbo.InsertCompany", commandtype: CommandType.StoredProcedure)]
    public class InsertCompanyByProcedure
    {
        [DBParameter("id", Direction = ParameterDirection.Output)]
        public int Id { get; set; }

        [DBParameter("name")]
        public string Name { get; set; }

        [DBParameter("Adress")]
        public string Adress { get; set; }

        [DBParameter("Country")]
        public string Country { get; set; }

        [DBParameter("ZipCode")]
        public string ZipCode { get; set; }

        [DBParameter("HouseNumber")]
        public string HouseNumber { get; set; }

        [DBParameter("CompanyType")]
        public CompanyType CompanyType { get; set; }  // Enums translates to sql int.

        [DBParameter("EncryptedText", Encrypt = true)]
        public string EncryptedText { get; set; }
    }



    [DBCommand(@"insert into dbo.Company ([name], Adress, Country, ZipCode, HouseNumber, CompanyType, EncryptedText) 
                    values (@name, @Adress, @Country, @ZipCode, @HouseNumber, @CompanyType, @EncryptedText);
	                set @OutputId = @@IDENTITY
", commandtype: CommandType.Text)]
    public class InsertCompanyBySqlStatement
    {
        [DBParameter("OutputId", Direction = ParameterDirection.Output)]
        public int Id { get; set; }

        [DBParameter("name")]
        public string Name { get; set; }

        [DBParameter("Adress")]
        public string Adress { get; set; }

        [DBParameter("Country")]
        public string Country { get; set; }

        [DBParameter("ZipCode")]
        public string ZipCode { get; set; }

        [DBParameter("HouseNumber")]
        public string HouseNumber { get; set; }

        [DBParameter("CompanyType")]
        public CompanyType CompanyType { get; set; }  // Enums translates to sql int.

        [DBParameter("EncryptedText", Encrypt = true)]
        public string EncryptedText { get; set; }
    }



    [DBCommand(@"insert into dbo.Company ([name], Adress, Country, ZipCode, HouseNumber, CompanyType, EncryptedText) 
                    values (@name, @Adress, @Country, @ZipCode, @HouseNumber, @CompanyType, @EncryptedText);
	             set @OutputId = @@IDENTITY 
", commandtype: CommandType.Text)]
    public class InsertCompanyRandom
    {
        [DBParameter("OutputId", Direction = ParameterDirection.Output)]
        public int Id { get; set; }

        [DBRandomParameter("name")]
        public string Name { get; set; }

        [DBRandomParameter("Adress")]
        public string Adress { get; set; }

        [DBRandomParameter("Country")]
        public string Country { get; set; }

        [DBRandomParameter("ZipCode")]
        public string ZipCode { get; set; }

        [DBRandomParameter("HouseNumber")]
        public string HouseNumber { get; set; }

        [DBParameter("CompanyType")]
        public CompanyType CompanyType { get; set; } = CompanyType.BV;

        [DBRandomParameter("EncryptedText", Encrypt = true)]
        public string EncryptedText { get; set; }
    }


    [DBCommand(@"update dbo.Company ([Country]) set Country = @Country where Id > @id
", commandtype: CommandType.Text)]
    public class UpdateCompanyRandom
    {
        [DBParameter("Id")]
        public int Id { get; set; }

        [DBRandomParameter("Country", Items = new string[] { "Red", "Green", "Blue", "Yellow" } )]
        public string Country { get; set; }
    }
}
