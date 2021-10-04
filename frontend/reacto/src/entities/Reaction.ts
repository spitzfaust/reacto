import { ReactionType } from "./ReactionType";
import { Spectator } from "./Spectator";

export interface Reaction {
  id: string;
  spectator: Spectator;
  reactionType: ReactionType;
  createdAt: Date;
}
