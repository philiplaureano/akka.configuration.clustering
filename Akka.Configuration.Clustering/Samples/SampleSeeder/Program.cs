using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration.Clustering.ActorSystems;
using Akka.Event;
using static Akka.Configuration.Lambdas.ActorSystemHelpers;

namespace SampleSeeder
{
    public class SeederHost : ClusteredActorSystemHost
    {
        private readonly int _port;

        public SeederHost(int port)
        {
            _port = port;
        }

        protected override int GetNextAvailablePort()
        {
            return _port;
        }

        protected override IEnumerable<string> GetRoles(string systemName)
        {
            yield return "lighthouse";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var systemName = "MyActorSystem";
            var host = new SeederHost(8080);

            host.Run(systemName);
        }        
    }
}
