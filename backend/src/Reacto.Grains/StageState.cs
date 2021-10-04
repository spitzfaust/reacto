using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Reacto.Grains
{
    public record Reaction(Guid Id, Spectator Spectator, ReactionType ReactionType, int SortOrder, DateTimeOffset CreatedAt);

    public record Spectator
    {
        public Spectator(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            }

            Name = name;
        }

        public string Name { get; }
    }

    [Serializable]
    public class StageState
    {
        public string Name { get; set; } = "";

        public IDictionary<string, Spectator> ActiveSpectatorConnections { get; set; } = new Dictionary<string, Spectator>();
        public IImmutableSet<Spectator> ActiveSpectators => ActiveSpectatorConnections.Values.ToImmutableHashSet();
        public ISet<Spectator> InactiveSpectators { get; set; } = new HashSet<Spectator>();
        public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
    }
}
