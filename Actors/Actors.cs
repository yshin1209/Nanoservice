using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Actors.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            await this.StateManager.AddStateAsync(variable, value);
            return "Variable added";
        }

        async Task<string> IActors.SetValueAsync(string variable, dynamic value)
        {
            await this.StateManager.SetStateAsync(variable, value);
            return "Value updated";
        }

        async Task<dynamic> IActors.GetValueAsync(string variable)
        {
            dynamic value = await this.StateManager.GetStateAsync<dynamic>(variable);
            return value;
        }

        async Task<string> IActors.RemoveVariableAsync(string variable)
        {
            await this.StateManager.RemoveStateAsync(variable);
            return "Variable removed";
        }
    }
}
