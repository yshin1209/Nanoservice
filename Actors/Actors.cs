using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Actors.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;

namespace Actors
{

    [StatePersistence(StatePersistence.Persisted)]
    internal class Actors : Actor, IActors
    {

        public Actors(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actors Actor activated.");
            return base.OnActivateAsync();
        }

        async Task<string> IActors.AddVariableAsync(string variable, dynamic value)
        {
            try
            {
                await this.StateManager.TryAddStateAsync(variable, value);
                return "Variable added";
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        async Task<string> IActors.SetValueAsync(string variable, dynamic value)
        {
            try
            {
                await this.StateManager.SetStateAsync(variable, value);
                return "Value set";
            }
            catch (Exception e)
            {
                return e.ToString();
            }

        }

        async Task<dynamic> IActors.GetValueAsync(string variable)
        {
            try
            {
                var value = await this.StateManager.GetStateAsync<dynamic>(variable);
                return value;
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        async Task<string> IActors.RemoveVariableAsync(string variable)
        {
            try
            {
                await this.StateManager.RemoveStateAsync(variable);
                return "Variable removed";
            }
            catch (Exception e)
            {
                return e.ToString();
            }

        }
    }
}
