import { ReactionType } from "./ReactionType";

export default new Map<ReactionType, string>([
  [ReactionType.Happy, "🙂"],
  [ReactionType.Nerdy, "🤓"],
  [ReactionType.Dollars, "🤑"],
  [ReactionType.Sad, "😢"],
  [ReactionType.Angry, "👿"],
  [ReactionType.Poop, "💩"],
  [ReactionType.Clap, "👏"],
  [ReactionType.Beers, "🍻"],
]);
