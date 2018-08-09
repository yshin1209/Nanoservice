using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace Actors.Interfaces
{
    public interface IActors : IActor
    {
        Task<string> SetValueAsync(string variable, dynamic value);
        Task<dynamic> GetValueAsync(string variable);
        Task<string> RemoveVariableAsync(string variable);
    }
}
