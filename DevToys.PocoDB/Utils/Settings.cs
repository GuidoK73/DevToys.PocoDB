using System.Configuration;

namespace GK.Data
{
    public class DataSettings
    {

        private static DataSettings _instance;

        private DataSettings()
        {
        }

        public static DataSettings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DataSettings();
                return _instance;
            }
        }

        public bool StrictMapping { get; set; } = false;
    }
}