using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Newtonsoft.Json.Linq;

namespace Actors.Interfaces
{
    public interface IActors : IActor
    {
        Task<string> AddVariableAsync(string variable, dynamic value);
        Task<string> RemoveVariableAsync(string variable);
        Task<dynamic> GetValueAsync(string variable);
        Task<string> SetValueAsync(string variable, dynamic value);
    }
}
