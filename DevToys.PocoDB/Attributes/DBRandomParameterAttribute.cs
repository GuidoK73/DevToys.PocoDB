using DevToys.PocoDB.Enums;
using DevToys.PocoDB.RandomData;
using System;
using System.Data;
using System.Reflection;

namespace DevToys.PocoDB.Attributes
{
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

        public override void SetParameterValue<TCOMMAND>(TCOMMAND commandObject, PropertyInfo property, IDbDataParameter parameter)
        {
            // TODO:
            // Use Items when item array matches DataUtils.NetType

            object _value = property.GetValue(commandObject);
            parameter.ParameterName = Name;

            switch (DataUtils.GetNetType(property.PropertyType))
            {
                case DataUtils.NetType.Enum:
                    if (ItemsIsNetType(DataUtils.NetType.Enum))
                        _value = RandomHelper.RandomArrayItem(Items);
                    else
                        _value = RandomHelper.RandomEnum(property.PropertyType);
                    break;

                case DataUtils.NetType.String:
                    if (ItemsIsNetType(DataUtils.NetType.String))
                        _value = (String)RandomHelper.RandomArrayItem(Items);
                    else
                        _value = RandomString();
                    break;

                case DataUtils.NetType.Guid:
                    if (ItemsIsNetType(DataUtils.NetType.Guid))
                        _value = (Guid)RandomHelper.RandomArrayItem(Items);
                    else
                        _value = Guid.NewGuid();
                    break;

                case DataUtils.NetType.Boolean:
                    if (ItemsIsNetType(DataUtils.NetType.Boolean))
                        _value = (bool)RandomHelper.RandomArrayItem(Items);
                    else
                        _value = RandomHelper.RandomNumber(1, 2) == 1;
                    break;

                case DataUtils.NetType.DateTime:
                    if (ItemsIsNetType(DataUtils.NetType.DateTime))
                        _value = (DateTime)RandomHelper.RandomArrayItem(Items);
                    else
                        _value = RandomHelper.RandomDateTime(DateMin, DateMax);
                    break;

                case DataUtils.NetType.DateTimeOffset:
                case DataUtils.NetType.TimeSpan:
                    break;

                case DataUtils.NetType.Byte:
                    if (ItemsIsNetType(DataUtils.NetType.Byte))
                        _value = (Byte)RandomHelper.RandomArrayItem(Items);
                    else
                        _value = (Byte)RandomHelper.RandomDouble((double)Min, (double)Max);
                    break;

                case DataUtils.NetType.SByte:
                    if (ItemsIsNetType(DataUtils.NetType.SByte))
                        _value = (SByte)RandomHelper.RandomArrayItem(Items);
                    else
                        _value = (SByte)RandomHelper.RandomDouble((double)Min, (double)Max);
                    break;

                case DataUtils.NetType.Int16:
                    if (ItemsIsNetType(DataUtils.NetType.Int16))
                        _value = (Int16)RandomHelper.RandomArrayItem(Items);
                    else
                        _value = (Int16)RandomHelper.RandomDouble((double)Min, (double)Max);
                    break;

                case DataUtils.NetType.Int32:
                    if (ItemsIsNetType(DataUtils.NetType.Int32))
                        _value = (Int32)RandomHelper.RandomArrayItem(Items);
                    else
                        _value = RandomHelper.RandomNumber(Min, Max);
                    break;

                case DataUtils.NetType.Int64:
                    if (ItemsIsNetType(DataUtils.NetType.Int64))
                        _value = (Int64)RandomHelper.RandomArrayItem(Items);
                    else
                        _value = (Int64)RandomHelper.RandomDouble((double)Min, (double)Max);
                    break;

                case DataUtils.NetType.Single:
                    if (ItemsIsNetType(DataUtils.NetType.Single))
                        _value = (Single)RandomHelper.RandomArrayItem(Items);
                    else
                        _value = (Single)RandomHelper.RandomDouble((double)Min, (double)Max);
                    break;

                case DataUtils.NetType.Decimal:
                    if (ItemsIsNetType(DataUtils.NetType.Decimal))
                        _value = (Decimal)RandomHelper.RandomArrayItem(Items);
                    else
                        _value = (decimal)RandomHelper.RandomDouble((double)Min, (double)Max);
                    break;

                case DataUtils.NetType.Double:
                    if (ItemsIsNetType(DataUtils.NetType.Double))
                        _value = (Double)RandomHelper.RandomArrayItem(Items);
                    else
                        _value = (double)RandomHelper.RandomDouble((double)Min, (double)Max);
                    break;
            }

            parameter.Value = _value;
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