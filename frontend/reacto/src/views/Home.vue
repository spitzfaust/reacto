<template>
  <div class="row">
    <div class="col-12 col-md-6">
      <p>
        This is <strong>Reacto</strong>. You can join a stage with a user name
        and post reactions to the stage. You and all other spectators of a stage
        will see those reactions.
      </p>
    </div>
    <div class="col-12 col-md-6">
      <div class="input-group input-group-lg mb-3">
        <input
          type="text"
          class="form-control"
          placeholder="Enter stage name"
          v-model="stageName"
          aria-label="Enter stage name"
          @keypress.enter.exact="joinStage"
        />
        <button
          :disabled="!isStageNameValid"
          @click="joinStage"
          class="btn btn-primary"
        >
          Join Stage
        </button>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { computed, defineComponent, Ref, ref } from "vue";

export default defineComponent({
  name: "Home",
  setup() {
    const stageName: Ref<string> = ref("");
    const isStageNameValid: Ref<boolean> = computed(
      () => stageName.value != null && stageName.value.trim() != ""
    );

    return {
      stageName,
      isStageNameValid,
    };
  },
  methods: {
    async joinStage() {
      if (this.isStageNameValid) {
        this.$router.push({
          name: "stage",
          params: { name: this.stageName },
        });
      }
    },
  },
  components: {},
});
</script>
