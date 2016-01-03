using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Configuration.Lambdas;

namespace Akka.Configuration.Clustering.ActorSystems
{
    public abstract class ActorSystemHost : IActorSystemHost
    {
        public virtual void Run(string systemName)
        {
            var config = new Dictionary<string, string>();

            AddConfigurationSettings(config);

            Action<ActorSystem> installActors = actorSystem =>
            {
                InstallActors(actorSystem);
                AwaitTermination(actorSystem);
            };

            var host = ActorSystemHelpers.CreateHost(installActors, config);
            host.Run(systemName);
        }

        protected virtual void InstallActors(ActorSystem actorSystem)
        {
        }
        protected virtual void AddConfigurationSettings(IDictionary<string, string> configEntries)
        {
            Action<string, string> setKey = (key, value) => { configEntries[key] = value; };

            setKey("akka.stdout-loglevel", LogLevel);
            setKey("akka.loglevel", LogLevel);
            setKey("akka.log-config-on-start", "on");

            setKey("akka.actor.debug.receive", "on");
            setKey("akka.actor.debug.autoreceive", "on");
            setKey("akka.actor.debug.lifecycle", "on");
            setKey("akka.actor.debug.event-stream", "on");
            setKey("akka.actor.debug.unhandled", "on");
        }

        protected virtual string LogLevel => "DEBUG";

        protected virtual void AwaitTermination(ActorSystem actorSystem)
        {
            actorSystem.AwaitTermination();
        }
    }
}