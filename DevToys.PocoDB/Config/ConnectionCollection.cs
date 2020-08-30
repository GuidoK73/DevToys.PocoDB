using System.Configuration;

namespace DevToys.PocoDB
{
    public class ConnectionCollection : ConfigurationElementCollection
    {
        public ConnectionCollection() 
        { 
        }

        public ConnectionConfig this[int index]
        {
            get { return (ConnectionConfig)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);

                BaseAdd(index, value);
            }
        }

        public void Add(ConnectionConfig serviceConfig) => BaseAdd(serviceConfig);

        public void Clear() => BaseClear();

        protected override ConfigurationElement CreateNewElement() => new ConnectionConfig();

        protected override object GetElementKey(ConfigurationElement element) => ((ConnectionConfig)element).Name;

        public void Remove(ConnectionConfig serviceConfig) => BaseRemove(serviceConfig.Name);

        public void RemoveAt(int index) => BaseRemoveAt(index);

        public void Remove(string name) => BaseRemove(name);
    }
}
