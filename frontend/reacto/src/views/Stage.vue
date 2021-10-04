<template>
  <div>
    <h1>Reacto</h1>
    <h2>Stage: {{ name }}</h2>
    <div v-if="showLogin">
      <label for="spectatorName">Please enter your name:</label>
      <input id="spectatorName" type="text" v-model.trim="spectatorInput" />
      <button @click="addSpectator" :disabled="!isSpectatorValid">Join</button>
    </div>
    <div v-else-if="showLoading">Joining stage, please hold on...</div>
    <div v-else>
      <p
        v-if="
          sharedState.stageSpectators[name] != null &&
          sharedState.stageSpectators[name] != ''
        "
      >
        logged in as {{ sharedState.stageSpectators[name] }}
      </p>
      <ReactionInput @click="react" />
      <div v-for="r in reactions" :key="r.id">
        {{ mapping.get(r.reactionType) }} - {{ r.spectator.name }}
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { computed, defineComponent, onMounted, Ref, ref } from "vue";
import { HubConnectionBuilder } from "@microsoft/signalr";
import { LoadingState } from "@/entities/LoadingState";
import ReactionInput from "@/components/ReactionInput.vue";
import store from "@/store";
import { ReactionType } from "@/entities/ReactionType";
import { Reaction } from "@/entities/Reaction";
import mapping from "@/entities/ReactionEmojiMapping";

export default defineComponent({
  name: "Stage",
  setup(props) {
    const spectatorInput = ref("");
    const isSpectatorValid = computed(
      () => spectatorInput.value != null && spectatorInput.value != ""
    );
    const joinStageState = ref(LoadingState.None);

    let connection = new HubConnectionBuilder()
      .withUrl("https://localhost:5001/stage")
      .build();

    const reactions: Ref<Reaction[]> = ref([]);

    connection.on("ReceiveReaction", (r: Reaction) => {
      reactions.value.push(r);
    });

    connection.on("ReceiveAllReactions", (r: Reaction[]) => {
      reactions.value = r;
    });

    const addSpectator = async () => {
      joinStageState.value = LoadingState.Loading;
      await connection.invoke("joinStage", {
        stageName: props.name,
        spectatorName: spectatorInput.value,
      });
      store.addStageSpectator(props.name, spectatorInput.value);
      joinStageState.value = LoadingState.Success;
    };

    const initializeConnection = async () => {
      await connection.start();
      if (!store.state.stageSpectators[props.name]) {
        return;
      }
      spectatorInput.value = store.state.stageSpectators[props.name];
      await addSpectator();
    };

    onMounted(initializeConnection);

    const showLogin = computed(
      () =>
        joinStageState.value == LoadingState.None ||
        joinStageState.value == LoadingState.Failure
    );

    const showLoading = computed(
      () => joinStageState.value == LoadingState.Loading
    );

    const react = async (reactionType: ReactionType) => {
      console.log(reactionType);
      await connection.send("postReaction", {
        reactionType: reactionType,
      });
    };

    return {
      connection,
      sharedState: store.state,
      spectatorInput,
      addSpectator,
      isSpectatorValid,
      joinStageState,
      showLogin,
      showLoading,
      react,
      reactions,
      mapping,
    };
  },
  props: {
    name: { type: String, required: true },
  },
  components: { ReactionInput },
});
</script>
