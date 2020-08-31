drop table dbo.Company
go
 
drop procedure dbo.InsertCompany
go

drop procedure dbo.GetCompany
go

drop procedure dbo.GetCompanies
go

drop table dbo.BinaryData
go


Create table dbo.Company
(
	Id int not null identity(1,1) primary key,
	[Name] varchar(128) not null,
	Adress varchar(128) null,
	Country varchar(128) not null,
	ZipCode varchar(16) not null,
	HouseNumber varchar(16) not null,
	Text varchar(256) null,
	CompanyType int not null default 1,
)
go


create table dbo.BinaryData
(
	Id int not null identity(1,1) primary key,
	[Name] varchar(128) not null,
	Photo image null,
	Document binary null
)
go


Create procedure dbo.InsertCompany(
	@name varchar(128), 
	@Adress varchar(128), 
	@Country varchar(128), 
	@ZipCode varchar(16), 
	@HouseNumber varchar(16), 
	@CompanyType int,
	@Text varchar(256),
	@id int out)
as
begin
	insert into dbo.Company ([name], Adress, Country, ZipCode, HouseNumber, CompanyType, Text) values (@name, @Adress, @Country, @ZipCode, @HouseNumber, @CompanyType, @Text)
	set @id = @@IDENTITY

end

go

create procedure dbo.GetCompany(@id int)
as
begin
	select id, [name], Adress, Country, ZipCode, HouseNumber, CompanyType, Text from dbo.Company where id = @id;
end
go


create procedure dbo.GetCompanies(@ids varchar(100))
as
begin
	select id, [name], Adress, Country, ZipCode, HouseNumber, CompanyType, Text from dbo.Company where id in (select convert(int, [value]) from STRING_SPLIT ( @ids, ','))  ;
end
go


