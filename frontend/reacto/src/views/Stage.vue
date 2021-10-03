<template>
  <div>
    <h1>Reacto</h1>
    <h2>
      Stage: {{ name }}
      <small
        v-if="
          sharedState.stageSpectators[name] != null &&
          sharedState.stageSpectators[name] != ''
        "
        >logged in as {{ sharedState.stageSpectators[name] }}</small
      >
    </h2>
    <div v-if="showLogin">
      <input type="text" v-model.trim="spectatorInput" />
      <button @click="addSpectator" :disabled="!isSpectatorValid">Join</button>
    </div>
  </div>
</template>

<script lang="ts">
import { computed, defineComponent, onMounted, ref } from "vue";
import { HubConnectionBuilder } from "@microsoft/signalr";
import { LoadingState } from "../entities/LoadingState";
import store from "../store";

export default defineComponent({
  name: "Stage",
  setup(props) {
    let connection = new HubConnectionBuilder()
      .withUrl("https://localhost:5001/stage")
      .build();

    const initializeConnection = async () => {
      await connection.start();
    };

    onMounted(initializeConnection);
    const spectatorInput = ref("");
    const isSpectatorValid = computed(
      () => spectatorInput.value != null && spectatorInput.value != ""
    );

    const addSpectator = async () => {
      joinStageState.value = LoadingState.Loading;
      await connection.send("joinStage", {
        stageName: props.name,
        spectatorName: spectatorInput.value,
      });
      store.addStageSpectator(props.name, spectatorInput.value);
      joinStageState.value = LoadingState.Success;
    };

    const joinStageState = ref(LoadingState.None);

    const showLogin = computed(
      () =>
        joinStageState.value == LoadingState.None ||
        joinStageState.value == LoadingState.Failure
    );

    return {
      connection,
      sharedState: store.state,
      spectatorInput,
      addSpectator,
      isSpectatorValid,
      joinStageState,
      showLogin,
    };
  },
  props: {
    name: { type: String, required: true },
  },
});
</script>
