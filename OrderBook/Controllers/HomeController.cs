using Contracts;
using Microsoft.AspNetCore.Mvc;
using OrderBook.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace OrderBook.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("send_data")]
        public async Task<string> send_data()
        {
            string uri = "net.tcp://DESKTOP-DP55SKQ:3001/ServiceEndpoint";
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);

            var channel = new ChannelFactory<IValidation>(binding, new EndpointAddress(uri));

            var proxy = channel.CreateChannel();
            bool result = await proxy.check_something("petar", "book", 3);
            if (result)
                return "Validno";
            return "Nije validno";
        }
    }
}
