<template>
  <div>
    <button @click="$emit('click', m[0])" v-for="m in mapping" :key="m[0]">
      {{ m[1] }}
    </button>
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
