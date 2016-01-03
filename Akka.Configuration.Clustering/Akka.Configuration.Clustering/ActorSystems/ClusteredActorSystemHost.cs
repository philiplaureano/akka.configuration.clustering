using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Akka.Configuration.Clustering.ActorSystems
{
    public abstract class ClusteredActorSystemHost : ActorSystemHost
    {
        protected string SystemName { get; set; }

        public override void Run(string systemName)
        {
            SystemName = systemName;
            base.Run(systemName);
        }

        protected override void AddConfigurationSettings(IDictionary<string, string> entries)
        {
            base.AddConfigurationSettings(entries);

            Action<string, string> setKey = (key, value) => { entries[key] = value; };

            var systemName = SystemName;
            var currentIpAddress = GetLocalIPAddress();
            var port = GetNextAvailablePort();

            setKey("akka.actor.provider", "\"Akka.Cluster.ClusterActorRefProvider, Akka.Cluster\"");
            setKey("akka.remote.log-remote-lifecycle-events", LogLevel);

            setKey("akka.remote.helios.tcp.public-hostname", currentIpAddress);
            setKey("akka.remote.helios.tcp.hostname", currentIpAddress);
            setKey("akka.remote.helios.tcp.port", port.ToString());

            setKey("akka.cluster.failure-detector.heartbeat-interval", "1s");

            // Determine the seed nodes
            var seedNodeList = (GetSeedNodes(systemName, currentIpAddress, port) ?? new string[0])
                .Distinct();

            var seedNodesValue = $"[{string.Join(",", seedNodeList.Select(address => $"\"{address}\""))}]";

            setKey("akka.cluster.seed-nodes", seedNodesValue);

            // Determine the roles for the current node
            var roles = (GetRoles(systemName) ?? new string[0]).Distinct();
            var rolesValue = $"[{string.Join(",", roles.Select(address => $"{address}"))}]";
            setKey("akka.cluster.roles", rolesValue);
        }

        protected virtual IEnumerable<string> GetSeedNodes(string systemName, string currentIpAddress, int port)
        {
            yield return $"akka.tcp://{systemName}@{currentIpAddress}:{port}";
        }

        protected virtual IEnumerable<string> GetRoles(string systemName)
        {
            yield break;
        }

        protected virtual string GetLocalIPAddress()
        {
            return Dns.GetHostEntry(Dns.GetHostName())
                .AddressList
                .FirstOrDefault(o => o.AddressFamily == AddressFamily.InterNetwork)?
                .ToString();
        }

        protected abstract int GetNextAvailablePort();
    }
}