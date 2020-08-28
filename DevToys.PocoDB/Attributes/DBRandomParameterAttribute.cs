using DevToys.PocoDB.Encryption;
using DevToys.PocoDB.RandomData;
using System;
using System.Data;
using System.Reflection;
using System.Security;

namespace DevToys.PocoDB.Attributes
{ 
    public enum RandomStringType
    {
        /// <summary>
        /// Random Name out of 500 first names
        /// </summary>
        FirstName = 0,
        /// <summary>
        /// Random Name out of 500 Last names
        /// </summary>
        LastName = 1,
        /// <summary>
        /// First name + Last name
        /// </summary>
        FullName = 2,
        /// <summary>
        /// Random word out of 500 words
        /// </summary>
        Word = 3,
        /// <summary>
        /// A text of random words (number of words limited by Max property)
        /// </summary>
        Text = 4,
        /// <summary>
        /// Random Company name out of 500 company namrs
        /// </summary>
        CompanyName = 5,
        /// <summary>
        /// Company name formatted as an URL
        /// </summary>
        Url = 6,
        /// <summary>
        /// Random Number between Min and Max
        /// </summary>
        Number = 7,
        /// <summary>
        /// Random Guid
        /// </summary>
        Guid = 8,
        /// <summary>
        /// @U = Uppercase char @L Lowercase char @N numeric char
        /// </summary>
        Format = 9,
        /// <summary>
        /// 
        /// </summary>
        Password = 10,
        /// <summary>
        /// Random Date between DateMin and DateMax
        /// </summary>
        DateTime = 11,
        /// <summary>
        /// Random Date between DateMin and DateMax, formatted with Format property
        /// </summary>
        DateTimeFormatted = 12,
        /// <summary>
        /// Random Country name out of 500 names
        /// </summary>
        Country = 13,
        /// <summary>
        /// Random dutch street name out of 500
        /// </summary>
        Street = 14,
        /// <summary>
        /// Country,  City, street, housenumber, ZipCode
        /// </summary>
        Adress = 15,       
        /// <summary>
        /// Random generated zipcode
        /// </summary>
        ZipCode = 16,
        /// <summary>
        /// Random Color name
        /// </summary>
        ColorName = 17,
        /// <summary>
        /// Random 11 proof BSN number.
        /// </summary>
        BSNNumber = 18,
        /// <summary>
        /// Random item out of item array 
        /// </summary>
        Item = 19

    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class DBRandomParameterAttribute : DBParameterAttribute
    {
        public int PercentageNull = 0;

        /// <param name="name">DbParameter Name</param>
        public DBRandomParameterAttribute(string name) : base(name) { }

        /// <summary>
        /// Items will be used for property when property type matches Items type.
        /// </summary>
        public object[] Items { get; set; }

        /// <summary>
        /// For all random DateTime
        /// </summary>
        public DateTime DateMax { get; set; } = DateTime.MaxValue;

        /// <summary>
        /// For all random DateTime
        /// </summary>
        public DateTime DateMin { get; set; } = DateTime.MinValue;

        /// <summary>
        /// for RandomStringType.Format and RandomStringType.DateTime
        /// </summary>
        public string Format { get; set; } = string.Empty;

        /// <summary>
        /// For all numerics and RandomStringType.Number, RandomStringType.Text, Password
        /// </summary>
        public Int32 Max { get; set; } = Int32.MaxValue;

        /// <summary>
        /// For all numerics and RandomStringType.Number
        /// </summary>
        public Int32 Min { get; set; } = Int32.MinValue;

        public RandomStringType RandomStringType { get; set; } = RandomStringType.Word;


        private bool ItemsIsNetType(DataUtils.NetType expectedType)
        {
            if (Items == null)
                return false;

            Type type = Items.GetType().GetElementType();
            return DataUtils.GetNetType(type) == expectedType;
        }

        public override void SetParameterValue<TCOMMAND>(TCOMMAND commandObject, PropertyInfo property, IDbDataParameter parameter, SecureString password)
        { 
            // TODO:
            // Use Items when item array matches DataUtils.NetType

            object value = property.GetValue(commandObject);
            parameter.ParameterName = Name;

            switch (DataUtils.GetNetType(property.PropertyType))
            {               
                case DataUtils.NetType.Enum:
                    if (ItemsIsNetType(DataUtils.NetType.Enum))
                        value = RandomHelper.RandomArrayItem(Items);
                    else
                        value = RandomHelper.RandomEnum(property.PropertyType);
                    break;

                case DataUtils.NetType.String:
                    if (ItemsIsNetType(DataUtils.NetType.String))
                        value = (String)RandomHelper.RandomArrayItem(Items);
                    else
                        value = RandomString();
                    break;

                case DataUtils.NetType.Guid:
                    if (ItemsIsNetType(DataUtils.NetType.Guid))
                        value = (Guid)RandomHelper.RandomArrayItem(Items);
                    else
                        value = Guid.NewGuid();
                    break;

                case DataUtils.NetType.Boolean:
                    if (ItemsIsNetType(DataUtils.NetType.Boolean))
                        value = (bool)RandomHelper.RandomArrayItem(Items);
                    else
                        value = RandomHelper.RandomNumber(1, 2) == 1;
                    break;

                case DataUtils.NetType.DateTime:
                    if (ItemsIsNetType(DataUtils.NetType.DateTime))
                        value = (DateTime)RandomHelper.RandomArrayItem(Items);
                    else
                        value = RandomHelper.RandomDateTime(DateMin, DateMax);
                    break;

                case DataUtils.NetType.DateTimeOffset:
                case DataUtils.NetType.TimeSpan:
                    break;

                case DataUtils.NetType.Byte:
                    if (ItemsIsNetType(DataUtils.NetType.Byte))
                        value = (Byte)RandomHelper.RandomArrayItem(Items);
                    else
                        value = (Byte)RandomHelper.RandomDouble((double)Min, (double)Max);
                    break;

                case DataUtils.NetType.SByte:
                    if (ItemsIsNetType(DataUtils.NetType.SByte))
                        value = (SByte)RandomHelper.RandomArrayItem(Items);
                    else
                        value = (SByte)RandomHelper.RandomDouble((double)Min, (double)Max);
                    break;

                case DataUtils.NetType.Int16:
                    if (ItemsIsNetType(DataUtils.NetType.Int16))
                        value = (Int16)RandomHelper.RandomArrayItem(Items);
                    else
                        value = (Int16)RandomHelper.RandomDouble((double)Min, (double)Max);
                    break;

                case DataUtils.NetType.Int32:
                    if (ItemsIsNetType(DataUtils.NetType.Int32))
                        value = (Int32)RandomHelper.RandomArrayItem(Items);
                    else
                        value = RandomHelper.RandomNumber(Min, Max);
                    break;

                case DataUtils.NetType.Int64:
                    if (ItemsIsNetType(DataUtils.NetType.Int64))
                        value = (Int64)RandomHelper.RandomArrayItem(Items);
                    else
                        value = (Int64)RandomHelper.RandomDouble((double)Min, (double)Max);
                    break;

                case DataUtils.NetType.Single:
                    if (ItemsIsNetType(DataUtils.NetType.Single))
                        value = (Single)RandomHelper.RandomArrayItem(Items);
                    else
                        value = (Single)RandomHelper.RandomDouble((double)Min, (double)Max);
                    break;

                case DataUtils.NetType.Decimal:
                    if (ItemsIsNetType(DataUtils.NetType.Decimal))
                        value = (Decimal)RandomHelper.RandomArrayItem(Items);
                    else
                        value = (decimal)RandomHelper.RandomDouble((double)Min, (double)Max);
                    break;

                case DataUtils.NetType.Double:
                    if (ItemsIsNetType(DataUtils.NetType.Double))
                        value = (Double)RandomHelper.RandomArrayItem(Items);
                    else
                        value = (double)RandomHelper.RandomDouble((double)Min, (double)Max);
                    break;
            }

            parameter.Value = FieldEncryption.Encrypt(value, password, Encrypt);
        }

        private string RandomString()
        {
            switch (RandomStringType)
            {
                case RandomStringType.Word:
                    return RandomHelper.RandomWord();

                case RandomStringType.Text:
                    return RandomHelper.RandomText(Max);

                case RandomStringType.FullName:
                    return RandomHelper.RandomFullName();

                case RandomStringType.Street:
                    return RandomHelper.RandomStreet();

                case RandomStringType.ZipCode:
                    return RandomHelper.RandomZipCode();

                case RandomStringType.Password:
                    return RandomHelper.RandomPassword(Max > 15 ? 15 : Max < 8 ? 8 : Max).ToString();

                case RandomStringType.Number:
                    return RandomHelper.RandomNumber(Min, Max).ToString();

                case RandomStringType.LastName:
                    return RandomHelper.RandomLastName();

                case RandomStringType.Guid:
                    return RandomHelper.RandomGuid().ToString();

                case RandomStringType.Format:
                    return RandomHelper.RandomFormatString(Format);

                case RandomStringType.DateTime:
                    return RandomHelper.RandomDateTime(DateMin, DateMax).ToString();

                case RandomStringType.DateTimeFormatted:
                    return RandomHelper.RandomDateTimeFormatted(DateMin, DateMax, string.IsNullOrEmpty(Format) ? "dd-MM-yyyy" : Format);

                case RandomStringType.Country:
                    return RandomHelper.RandomCountry();

                case RandomStringType.CompanyName:
                    return RandomHelper.RandomCompany();

                case RandomStringType.Url:
                    return RandomHelper.RandomUrl();

                case RandomStringType.Adress:
                    return RandomHelper.RandomAdress();

                case RandomStringType.ColorName:
                    return RandomHelper.RandomColorName();

                case RandomStringType.Item:
                    return RandomHelper.RandomArrayItem(Items).ToString();

                case RandomStringType.BSNNumber:
                    return RandomHelper.RandomBSNNumber();

  
            }

            return RandomHelper.RandomWord();
        }
    }
}