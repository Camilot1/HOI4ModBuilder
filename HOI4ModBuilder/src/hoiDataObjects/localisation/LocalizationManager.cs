using HOI4ModBuilder.hoiDataObjects.history.countries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.hoiDataObjects.localisation
{
    class LocalizationManager
    {
        private static Dictionary<string, string> localizationDictrionary = new Dictionary<string, string>();

        public static void Load(Settings settings)
        {

        }

        public static string GetLocalization(string key)
        {
            if (!localizationDictrionary.ContainsKey(key))
            {
                localizationDictrionary.Add(key, key);
                return key;
            }

            return localizationDictrionary[key];
        }
    }
}
