using System.Threading.Tasks;
using Orleans;
using Orleans.Runtime;

namespace Reacto.Grains
{
    public class ConnectionState
    {
        public string? StageName { get; set; }
    }
    
    public class Connection : Grain, IConnection
    {
        private readonly IPersistentState<ConnectionState> connectionState;

        public Connection([PersistentState(StateNames.Connection)] IPersistentState<ConnectionState> connectionState)
        {
            this.connectionState = connectionState;
        }
        
        public async Task SetStageName(string stageName)
        {
            connectionState.State.StageName = stageName;
            await connectionState.WriteStateAsync();
        }

        public Task<string?> GetStageName()
        {
            return Task.FromResult(connectionState.State.StageName);
        }
    }
}
