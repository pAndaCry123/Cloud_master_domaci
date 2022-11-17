using Contracts;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Validator
{
    public class Validator_service : IValidation
    {
        public async Task<bool> check_something(string user, string book, int count)
        {
            if (count > 0)
            {

                FabricClient fabric_client = new FabricClient();

                int partition_number = (await fabric_client.QueryManager.GetApplicationListAsync(new Uri("fabric:/BookApp/Stateful1"))).Count;
                var binding = WcfUtility.CreateTcpClientBinding();

                int index = 0;

                ServicePartitionClient<WcfCommunicationClient<ITransactions>> service_partition_client = new
                    ServicePartitionClient<WcfCommunicationClient<ITransactions>>(
                        new WcfCommunicationClientFactory<ITransactions>(clientBinding: binding),
                        new Uri("fabric:/BookApp/Stateful1"),
                        new ServicePartitionKey(0)); //ovde se menja kasnije

                
                return await service_partition_client.InvokeWithRetryAsync(client => client.Channel.prepare()); 
            }
            return false;

        }
    }
}
