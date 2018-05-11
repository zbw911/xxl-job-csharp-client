using System;

namespace JobClient
{
    public class RegistryParam
    {
        public RegistryParam()
        {

        }

        public RegistryParam(string registGroup, string appName, string executorAddress)
        {
            this.registGroup = registGroup;
            this.registryKey = appName;
            this.registryValue = executorAddress;
        }

        public string registGroup { get; set; }
        public string registryKey { get; set; }
        public string registryValue { get; set; }


        public override string ToString()
        {
            return "RegistryParam{" +
              "registGroup='" + registGroup + '\'' +
              ", registryKey='" + registryKey + '\'' +
              ", registryValue='" + registryValue + '\'' +
              '}';
        }
    }
}
