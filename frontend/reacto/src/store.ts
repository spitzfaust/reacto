import { reactive } from "vue";

export interface State {
  stageSpectators: Record<string, string>;
}

export interface Store {
  state: State;
  addStageSpectator(stage: string, spectator: string): void;
  removeStageSpectator(stage: string): void;
}

const store: Store = {
  state: reactive({
    stageSpectators: {},
  }),

  addStageSpectator(stage: string, spectator: string): void {
    this.state.stageSpectators[stage] = spectator;
  },

  removeStageSpectator(stage: string): void {
    delete this.state.stageSpectators[stage];
  },
};

export default store;
