using NUnit.Framework;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FuelSDK.Test
{
    [TestFixture]
    class FuelSDKConfigurationSectionTest
    {
        private readonly string emptyConfigFileName = "empty.config";
        private readonly string allPropertiesSetConfigFileName = "allPropertiesSet.config";
        private readonly string noPropertiesSetConfigFileName = "noPropertiesSet.config";
        private string configFilePath;

        [Test()]
        public void NoCustomConfigSection()
        {
            FuelSDKConfigurationSection section = GetCustomConfigurationSectionFromConfigFile(emptyConfigFileName);
            Assert.IsNull(section);
        }

        [Test()]
        public void MissingAppSignaturePropertyFromConfigSection()
        {
            FuelSDKConfigurationSection section = GetCustomConfigurationSectionFromConfigFile(noPropertiesSetConfigFileName);
            Assert.AreEqual(section.AppSignature, string.Empty);
        }

        [Test()]
        public void MissingClientIdPropertyFromConfigSection()
        {
            FuelSDKConfigurationSection section = GetCustomConfigurationSectionFromConfigFile(noPropertiesSetConfigFileName);
            Assert.AreEqual(section.ClientId, string.Empty);
        }

        [Test()]
        public void MissingClientSecretPropertyFromConfigSection()
        {
            FuelSDKConfigurationSection section = GetCustomConfigurationSectionFromConfigFile(noPropertiesSetConfigFileName);
            Assert.AreEqual(section.ClientSecret, string.Empty);
        }

        [Test()]
        public void MissingSoapEndPointPropertyFromConfigSection()
        {
            FuelSDKConfigurationSection section = GetCustomConfigurationSectionFromConfigFile(noPropertiesSetConfigFileName);
            Assert.AreEqual(section.SoapEndPoint, string.Empty);
        }

        [Test()]
        public void MissingAuthEndPointPropertyFromConfigSection()
        {
            FuelSDKConfigurationSection section = GetCustomConfigurationSectionFromConfigFile(noPropertiesSetConfigFileName);
            Assert.AreEqual(section.AuthenticationEndPoint, string.Empty);
        }

        [Test()]
        public void MissingRestEndPointPropertyFromConfigSection()
        {
            FuelSDKConfigurationSection section = GetCustomConfigurationSectionFromConfigFile(noPropertiesSetConfigFileName);
            Assert.AreEqual(section.RestEndPoint, string.Empty);
        }

        [Test()]
        public void AllPropertiesSetInConfigSection()
        {
            FuelSDKConfigurationSection section = GetCustomConfigurationSectionFromConfigFile(allPropertiesSetConfigFileName);
            Assert.AreEqual(section.AppSignature, "none");
            Assert.AreEqual(section.ClientId, "abc");
            Assert.AreEqual(section.ClientSecret, "cde");
            Assert.AreEqual(section.SoapEndPoint, "https://soapendpoint.com");
            Assert.AreEqual(section.AuthenticationEndPoint, "https://authendpoint.com");
            Assert.AreEqual(section.RestEndPoint, "https://restendpoint.com");
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(configFilePath))
            {
                File.Delete(configFilePath);
            }
        }

        private void CreateConfigFileFromResource(string resourceFileName)
        {
            Assembly a = Assembly.GetExecutingAssembly();
            using (Stream s = a.GetManifestResourceStream("FuelSDK.Test.ConfigFiles." + resourceFileName))
            {
                using (StreamReader sr = new StreamReader(s))
                {
                    configFilePath = Path.Combine(Path.GetTempPath(), resourceFileName);
                    using (StreamWriter sw = File.CreateText(configFilePath))
                    {
                        sw.Write(sr.ReadToEnd());
                        sw.Flush();
                    }
                }
            }
        }

        private FuelSDKConfigurationSection GetCustomConfigurationSectionFromConfigFile(string resourceFileName)
        {
            CreateConfigFileFromResource(resourceFileName);

            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = configFilePath;
            Configuration config
              = ConfigurationManager.OpenMappedExeConfiguration(fileMap,
                ConfigurationUserLevel.None);

            return config.GetSection("fuelSDK") as FuelSDKConfigurationSection;
        }
    }
}
