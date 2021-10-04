using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Orleans;
using Reacto.Grains;

namespace Reacto.Web.Hubs
{
    public class StageHub : Hub<IStageClient>
    {
        private readonly IClusterClient clusterClient;
        private readonly ILogger<StageHub> logger;

        public StageHub(IClusterClient clusterClient, ILogger<StageHub> logger)
        {
            this.clusterClient = clusterClient;
            this.logger = logger;
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
            await Clients.Caller.ReceiveAllReactions(await stage.GetReactions());
            await Groups.AddToGroupAsync(Context.ConnectionId, stageName);
        }

        public record PostReactionRequest
        {
            public PostReactionRequest(ReactionType reactionType)
            {
                if (!Enum.IsDefined(typeof(ReactionType), reactionType))
                {
                    throw new InvalidEnumArgumentException(nameof(reactionType), (int)reactionType, typeof(ReactionType));
                }

                ReactionType = reactionType;
            }

            public ReactionType ReactionType { get; }
        }

        public async Task PostReaction(PostReactionRequest postReactionRequest)
        {
            var connection = clusterClient.GetGrain<IConnection>(Context.ConnectionId);
            var stageName = await connection.GetStageName();
            if (stageName is null)
            {
                logger.LogWarning("Name of stage for connection cannot be found");
                return;
            }

            var stage = clusterClient.GetGrain<IStage>(stageName);
            var reaction = await stage.PostReaction(Context.ConnectionId, postReactionRequest.ReactionType);

            await Clients.Group(stageName).ReceiveReaction(reaction);
        }
    }
}
