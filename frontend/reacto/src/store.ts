import { reactive } from "vue";

export interface State {
  stageSpectators: Record<string, string>;
}

export interface Store {
  state: State;
  addStageSpectator(stage: string, spectator: string): void;
  removeStageSpectator(stage: string): void;
}

class StoreImpl implements Store {
  private readonly _state: State;
  private readonly persistentStateKey = "state";

  public get state() {
    return this._state;
  }

  constructor() {
    let state: State = {
      stageSpectators: {},
    };

    const persistedState = localStorage.getItem(this.persistentStateKey);
    if (persistedState) {
      state = JSON.parse(persistedState) as State;
    }

    this._state = reactive(state);
  }

  public addStageSpectator(stage: string, spectator: string): void {
    this.state.stageSpectators[stage] = spectator;
    this.persistState();
  }

  public removeStageSpectator(stage: string): void {
    delete this.state.stageSpectators[stage];
    this.persistState();
  }

  private persistState(): void {
    window.localStorage.setItem(
      this.persistentStateKey,
      JSON.stringify(this._state)
    );
  }
}

const store: Store = new StoreImpl();

export default store;
