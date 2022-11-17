using Contracts;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
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


namespace Stateful1
{
    public class Statefull_service : ITransactions
    {
        IReliableStateManager StateManager { get; set; }

        public Statefull_service(IReliableStateManager stateManager)
        {
            StateManager = stateManager;
        }

        public async Task Commit()
        {
            //Some stupid logic :D
            var commit_dict = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, string>>("myStateCommit");
            using (var tx = this.StateManager.CreateTransaction())
            {
                await commit_dict.TryAddAsync(tx, "1", "state 2");
                await tx.CommitAsync();
            }
        }

        public async Task<bool> prepare()
        {

            FabricClient fabric_client = new FabricClient();

            int partition_number = (await fabric_client.QueryManager.GetApplicationListAsync(new Uri("fabric:/BookApp/BookStore"))).Count;
            var binding = WcfUtility.CreateTcpClientBinding();

            int index = 0;

            ServicePartitionClient<WcfCommunicationClient<IBookStore>> service_partition_client = new
                ServicePartitionClient<WcfCommunicationClient<IBookStore>>(
                    new WcfCommunicationClientFactory<IBookStore>(clientBinding: binding),
                    new Uri("fabric:/BookApp/BookStore"),
                    new ServicePartitionKey(0)); //ovde se menja kasnije


            int partition_number1 = (await fabric_client.QueryManager.GetApplicationListAsync(new Uri("fabric:/BookApp/UserStore"))).Count;

            int index1 = 0;

            ServicePartitionClient<WcfCommunicationClient<IUserStore>> service_partition_client1 = new
                ServicePartitionClient<WcfCommunicationClient<IUserStore>>(
                    new WcfCommunicationClientFactory<IUserStore>(clientBinding: binding),
                    new Uri("fabric:/BookApp/UserStore"),
                    new ServicePartitionKey(0)); //ovde se menja kasnije

            //ovde commit samo provera treba da se obavi
            bool book = await service_partition_client.InvokeWithRetryAsync(client => client.Channel.prepare());
            bool user = await service_partition_client1.InvokeWithRetryAsync(client => client.Channel.prepare());
            if(book && user)
            {
                await Commit();
                return true;
            }
            else
            {
                await Rollback();
                return false;
            }

        }

        public async Task Rollback()
        {
           var rollb_dict = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, string>>("myStateRollback");
                using (var tx = this.StateManager.CreateTransaction())
                {
                    await rollb_dict.TryAddAsync(tx, "1", "ret_state 1");
                    await tx.CommitAsync();
                }
        }
    }
}
