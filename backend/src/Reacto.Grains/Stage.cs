using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;

namespace Reacto.Grains
{
    public class Stage : Grain, IStage
    {
        private readonly IPersistentState<StageState> stageState;
        private readonly IGrainFactory grainFactory;
        private readonly ILogger<Stage> logger;

        public Stage([PersistentState(StateNames.Stage)] IPersistentState<StageState> stageState,
            IGrainFactory grainFactory,
            ILogger<Stage> logger)
        {
            this.stageState = stageState;
            this.grainFactory = grainFactory;
            this.logger = logger;
        }

        /// <inheritdoc />
        public override Task OnActivateAsync()
        {
            stageState.State.Name = this.GetPrimaryKeyString();
            return stageState.WriteStateAsync();
        }

        public async Task<Reaction> PostReaction(Spectator spectator, ReactionType reactionType)
        {
            if (!Enum.IsDefined(typeof(ReactionType), reactionType))
            {
                throw new InvalidEnumArgumentException(nameof(reactionType), (int)reactionType, typeof(ReactionType));
            }

            if (!stageState.State.ActiveSpectators.Contains(spectator))
            {
                throw new InvalidOperationException($"Spectator {spectator} is not active");
            }

            var reaction = new Reaction(Guid.NewGuid(), spectator, reactionType, DateTimeOffset.Now);

            stageState.State.Reactions.Add(reaction);
            await stageState.WriteStateAsync();

            return reaction;
        }

        public async Task JoinStage(Spectator spectator, string connectionId)
        {
            if (string.IsNullOrWhiteSpace(connectionId))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionId));
            }

            logger.LogInformation("Trying to add spectator {@Spectator} to stage {StageName}", spectator, stageState.State.Name);

            var connection = grainFactory.GetGrain<IConnection>(connectionId);
            await connection.SetStageName(stageState.State.Name);

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

        public async Task<Reaction?> PostReaction(string connectionId, ReactionType reactionType)
        {
            if (!Enum.IsDefined(typeof(ReactionType), reactionType))
            {
                throw new InvalidEnumArgumentException(nameof(reactionType), (int)reactionType, typeof(ReactionType));
            }

            if (!stageState.State.ActiveSpectatorConnections.TryGetValue(connectionId, out var spectator))
            {
                logger.LogError("Connection {ConnectionId} is not active cannot post reaction {ReactionType}",
                    connectionId,
                    reactionType);

                return null;
            }

            var reaction = new Reaction(Guid.NewGuid(), spectator, reactionType, DateTimeOffset.Now);

            stageState.State.Reactions.Add(reaction);
            await stageState.WriteStateAsync();

            return reaction;
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
