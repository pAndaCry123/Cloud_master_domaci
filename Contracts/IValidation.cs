using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [ServiceContract]
    public interface IValidation
    {
        [OperationContract]
        Task<bool> check_something(string user, string book, int count);
    }
}
