using System;

namespace DevToys.PocoDB.Attributes
{
    public abstract class BaseDBParameterAttribute : Attribute
    {
        /// <param name="name">DbParameter Name</param>
        public BaseDBParameterAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// DB fieldname
        /// </summary>
        public string Name { get; private set; }
    }
}