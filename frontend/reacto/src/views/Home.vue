<template>
  <div class="join-stage">
    <div>
      <h1>Reacto</h1>
      <div>
        <input type="text" placeholder="Enter Stage name" v-model="stageName" />
        <button :disabled="!isStageNameValid" @click="joinStage">
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

<style lang="scss">
.join-stage {
  height: 100vh;
  display: flex;
}
</style>
