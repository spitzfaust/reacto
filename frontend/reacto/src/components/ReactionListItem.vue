<template>
  <li class="list-group-item">
    <div class="d-flex gap-3 align-items-center">
      <div class="fs-1">{{ reactionTypeEmoji }}</div>
      <div class="d-flex flex-column">
        <div style="font-weight: bold">{{ spectator }}</div>
        <div style="font-style: italic">{{ durationSince }}</div>
      </div>
    </div>
  </li>
</template>

<script lang="ts">
import { computed, defineComponent, onUnmounted, ref } from "vue";
import { formatDistance } from "date-fns";
import mapping from "@/entities/ReactionEmojiMapping";
import { enUS } from "date-fns/locale";

export default defineComponent({
  name: "ReactionInput",
  setup(props) {
    const reactionTypeEmoji = mapping.get(props.reaction.reactionType);
    const spectator = props.reaction.spectator.name as string;
    const createdAt = new Date(props.reaction.createdAt);
    const now = ref(Date.now());
    const intervalId = setInterval(() => (now.value = Date.now()), 5000);
    onUnmounted(() => clearInterval(intervalId));
    const durationSince = computed(() =>
      formatDistance(createdAt, now.value, {
        includeSeconds: true,
        addSuffix: true,
        locale: enUS,
      })
    );

    return {
      reactionTypeEmoji,
      spectator,
      createdAt,
      durationSince,
    };
  },
  props: {
    reaction: {
      type: Object,
      required: true,
    },
  },
});
</script>

<style lang="scss" scoped>
.reaction-input {
  background-color: #aa1155;
}
</style>
