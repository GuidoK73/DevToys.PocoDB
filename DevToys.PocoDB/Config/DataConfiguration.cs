using System.Collections.Generic;
using System.Configuration;

namespace DevToys.PocoDB
{
    public class DataConfiguration
    {
        private static DataConfiguration _Instance;

        private readonly Dictionary<string, ConnectionConfig> _Connections = new Dictionary<string, ConnectionConfig>();

        private DataConfiguration()
        {
            ConnectionConfigurationSection serviceConfigSection = ConfigurationManager.GetSection("DevToys.PocoDB") as ConnectionConfigurationSection;
            foreach (ConnectionConfig connection in serviceConfigSection.Connections)
                _Connections.Add(connection.Name.ToLower(), connection);
        }

        public static DataConfiguration Instance => _Instance ?? (_Instance = new DataConfiguration());

        public ConnectionConfig Get(string name)
        {
            if (!_Connections.ContainsKey(name.ToLower()))
                throw new DataException(string.Format("Could not find Data Connection named: '{0}'", name));

            return _Connections[name.ToLower()];
        }
    }
}