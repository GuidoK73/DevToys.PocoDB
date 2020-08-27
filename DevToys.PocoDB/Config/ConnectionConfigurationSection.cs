using System.Configuration;

namespace DevToys.PocoDB
{ 
    public class ConnectionConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("Connections", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ConnectionCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public ConnectionCollection Connections => (ConnectionCollection)base["Connections"];
    }
}