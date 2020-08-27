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
        /// Indicates whether this data must be Encrypted. (Only works for String and Byte[] properties).
        ///
        /// </summary>
        public bool Encrypt { get; set; } = false;

        /// <summary>
        /// DB fieldname
        /// </summary>
        public string Name { get; private set; }
    }
}