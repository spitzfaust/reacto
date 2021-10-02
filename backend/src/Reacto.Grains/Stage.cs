using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;

namespace Reacto.Grains
{
    public class Stage : Grain, IStage
    {
        private readonly IPersistentState<StageState> stageState;
        private readonly ILogger<Stage> logger;

        public Stage([PersistentState(StateNames.Stage)] IPersistentState<StageState> stageState, ILogger<Stage> logger)
        {
            this.stageState = stageState;
            this.logger = logger;
        }

        /// <inheritdoc />
        public override Task OnActivateAsync()
        {
            stageState.State.Name = this.GetPrimaryKeyString();
            return stageState.WriteStateAsync();
        }

        public Task<Reaction> PostReaction(Spectator spectator, ReactionType reactionType)
        {
            throw new NotImplementedException();
        }

        public async Task JoinStage(Spectator spectator, string connectionId)
        {
            if (string.IsNullOrWhiteSpace(connectionId))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionId));
            }

            logger.LogInformation("Trying to add spectator {@Spectator} to stage {StageName}", spectator, stageState.State.Name);

            if (stageState.State.ActiveSpectatorConnections.ContainsKey(connectionId))
            {
                logger.LogInformation("Spectator {@Spectator} tried to join stage {StageName} with the same connection id {ConnectionId}",
                    spectator,
                    stageState.State.Name,
                    connectionId);
                return;
            }

            stageState.State.InactiveSpectators.Remove(spectator);
            stageState.State.ActiveSpectatorConnections.Add(connectionId, spectator);
            await stageState.WriteStateAsync();
        }

        public Task LeaveStage(Spectator spectator)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Reaction>> GetReactions(Spectator spectator)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Spectator>> GetActiveSpectators()
        {
            return Task.FromResult<IEnumerable<Spectator>>(stageState.State.ActiveSpectators);
        }
    }
}
