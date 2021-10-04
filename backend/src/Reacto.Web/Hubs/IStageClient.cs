using System.Collections.Generic;
using System.Threading.Tasks;
using Reacto.Grains;

namespace Reacto.Web.Hubs
{
    public interface IStageClient
    {
        Task ReceiveReaction(Reaction reaction);

        Task ReceiveAllReactions(IEnumerable<Reaction> reactions);
    }
}
