using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;

namespace Reacto.Grains
{
    public interface IStage : IGrainWithStringKey
    {
        /// <summary>
        /// Add the <paramref name="spectator"/> with the <paramref name="connectionId"/> to the dictionary of spectators.
        /// </summary>
        /// <param name="spectator"></param>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public Task JoinStage(Spectator spectator, string connectionId);

        public Task LeaveStage(Spectator spectator);
        
        public Task<Reaction?> PostReaction(string connectionId, ReactionType reactionType);

        public Task<IEnumerable<Reaction>> GetReactions();

        public Task<IEnumerable<Spectator>> GetActiveSpectators();
    }
}
