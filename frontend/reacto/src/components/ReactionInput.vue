<template>
  <div class="row reaction-input">
    <div
      class="col-3 col-md my-3 d-flex justify-content-center"
      v-for="m in mapping"
      :key="m[0]"
    >
      <button class="btn btn-lg btn-info shadow" @click="$emit('click', m[0])">
        {{ m[1] }}
      </button>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import mapping from "@/entities/ReactionEmojiMapping";

export default defineComponent({
  name: "ReactionInput",
  setup() {
    return {
      mapping,
    };
  },
  props: {
    msg: String,
  },
  emits: ["click"],
});
</script>

<style lang="scss" scoped>
.reaction-input {
  background-color: #aa1155;
}
</style>
