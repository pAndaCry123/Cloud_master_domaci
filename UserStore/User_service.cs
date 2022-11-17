using Contracts;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserStore
{

    class User_service : IUserStore
    {
        IReliableStateManager StateManager { get; set; }
        string user1 { get; set; }
        double amount { get; set; }
        public User_service(IReliableStateManager stateManager)
        {
            StateManager = stateManager;
        }

    
        public async Task<User> get_user()
        {
            try
            {
                var user_dict = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, User>>("users");

                using (var tx = this.StateManager.CreateTransaction())
                {
                    var user = await user_dict.TryGetValueAsync(tx, "1");


                    await tx.CommitAsync();
                    return user.Value;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<bool> prepare()
        {
            this.user1 = "1";
            this.amount = 50.5;
            User user = await get_user();

            if (user != null && user.Amount > this.amount)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Task Commit()
        {
            throw new NotImplementedException();
        }

        public Task Rollback()
        {
            throw new NotImplementedException();
        }
    }
}
