<template>
  <div class="row">
    <div class="col">
      <h2>Stage: {{ name }}</h2>
    </div>
  </div>
  <div v-if="showLogin" class="row">
    <div class="col-12 col-md-6">
      <p>
        After you enter your name you will join the stage and be able to post
        your reactions.
      </p>
      <div class="input-group input-group-lg mb-3">
        <input
          type="text"
          class="form-control"
          placeholder="Enter your name"
          v-model.trim="spectatorInput"
          aria-label="Enter your name"
          @keypress.enter.exact="addSpectator"
        />
        <button
          @click="addSpectator"
          :disabled="!isSpectatorValid"
          class="btn btn-primary"
        >
          Join Stage
        </button>
      </div>
    </div>
  </div>
  <div v-else-if="showLoading">
    <div class="spinner-border text-primary" role="status">
      <span class="visually-hidden"> Joining stage, please hold on...</span>
    </div>
  </div>
  <div v-else class="row">
    <div class="col">
      <div class="row">
        <div class="col">
          <p
            v-if="
              sharedState.stageSpectators[name] != null &&
              sharedState.stageSpectators[name] != ''
            "
          >
            logged in as
            <strong>{{ sharedState.stageSpectators[name] }}</strong>
          </p>
        </div>
      </div>
      <ReactionInput @click="react" />
      <div class="row mt-2 mb-4 flex-grow-1">
        <div class="col">
          <div v-if="reactions.length == 0" class="alert alert-info">
            Reactions will be shown here as soon as they happen.
          </div>
          <ol v-else class="list-group">
            <ReactionListItem
              v-for="r in orderedReactions"
              :key="r.id"
              :reaction="r"
            />
          </ol>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { computed, defineComponent, onMounted, Ref, ref } from "vue";
import { HubConnectionBuilder } from "@microsoft/signalr";
import { LoadingState } from "@/entities/LoadingState";
import ReactionInput from "@/components/ReactionInput.vue";
import ReactionListItem from "@/components/ReactionListItem.vue";
import store from "@/store";
import { ReactionType } from "@/entities/ReactionType";
import mapping from "@/entities/ReactionEmojiMapping";
import { Reaction } from "@/entities/Reaction";

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

    const orderedReactions = computed(() =>
      reactions.value.slice().sort((a, b) => b.sortOrder - a.sortOrder)
    );

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
      orderedReactions,
      mapping,
    };
  },
  props: {
    name: { type: String, required: true },
  },
  components: { ReactionInput, ReactionListItem },
});
</script>
