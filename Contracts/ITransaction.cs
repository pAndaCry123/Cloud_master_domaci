using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [ServiceContract]
    public interface ITransactions
    {
        [OperationContract]
        Task Commit();

        [OperationContract]

        Task<bool> prepare();
        [OperationContract]
        Task Rollback();
    }
}
