using System.Configuration;
using System.IO;
using System.Reflection;

namespace FuelSDK
{
    class ConfigUtil
    {
        public static FuelSDKConfigurationSection GetFuelSDKConfigSection()
        {
#if NET40
            return (FuelSDKConfigurationSection)ConfigurationManager.GetSection("fuelSDK");
#else
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            Assembly assembly = Assembly.GetExecutingAssembly();
            fileMap.ExeConfigFilename = Path.Combine(new FileInfo(assembly.Location).Directory.FullName, "App.config");
            Configuration config =
                ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            return (FuelSDKConfigurationSection)config.GetSection("fuelSDK");
#endif
        }
    }
}
