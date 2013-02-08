using System.Globalization;
using System.Threading;
using NServiceBus.Timeout.Hosting.Azure;
using log4net.Core;
using NServiceBus;
using NServiceBus.Config;
using NServiceBus.Integration.Azure;
using StructureMap;

namespace Cashier
{
    public class Bootstrapper
    {
        private Bootstrapper()
        {}

        public static void Bootstrap()
        {
            BootstrapStructureMap();
            BootstrapNServiceBus();
        }

        private static void BootstrapStructureMap()
        {
            ObjectFactory.Initialize(x => x.AddRegistry(new CashierRegistry()));
        }

        private static void BootstrapNServiceBus()
        {
            Configure.Transactions.Enable();

            Configure.With()
                .Log4Net()
                .StructureMapBuilder(ObjectFactory.Container)
                .AzureMessageQueue().JsonSerializer()
                .AzureSubcriptionStorage()
                .Sagas().AzureSagaPersister()
                .UseAzureTimeoutPersister().ListenOnAzureStorageQueues()
                .UnicastBus()
                .LoadMessageHandlers()
                .CreateBus()
                .Start();
        }
    }
}
