using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Orleans;
using Reacto.Grains;

namespace Reacto.Web.Hubs
{
    public class StageHub : Hub<IStageClient>
    {
        private readonly IClusterClient clusterClient;

        public StageHub(IClusterClient clusterClient)
        {
            this.clusterClient = clusterClient;
        }

        public record JoinStageRequest(string StageName, string SpectatorName);

        public async Task JoinStage(JoinStageRequest joinStageRequest)
        {
            var (stageName, spectatorName) = joinStageRequest;
            if (string.IsNullOrEmpty(stageName))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(joinStageRequest));
            }

            if (string.IsNullOrEmpty(spectatorName))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(joinStageRequest));
            }

            var stage = clusterClient.GetGrain<IStage>(stageName);
            await stage.JoinStage(new Spectator(spectatorName), Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, stageName);
        }
    }
}
