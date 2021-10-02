<template>
  <div class="home">
    <h1>Chat</h1>
    <input type="text" v-model="name" />
    <input type="text" v-model="newMessage" />
    <button @click="sendMessage" :disabled="!isNewMessageValid">Send</button>
    <ul>
      <li v-for="message in messages" :key="message.id">
        {{ message }}
      </li>
    </ul>
  </div>
</template>

<script lang="ts">
import { computed, defineComponent, onMounted, Ref, ref } from "vue";
import { HubConnectionBuilder } from "@microsoft/signalr";

interface message {
  id: string;
  name: string;
  text: string;
}

export default defineComponent({
  name: "Home",
  setup() {
    let connection = new HubConnectionBuilder()
      .withUrl("https://localhost:5001/stage")
      .build();
    const messages: Ref<message[]> = ref([]);
    const name: Ref<string> = ref("");
    const newMessage: Ref<string> = ref("");
    const isNewMessageValid: Ref<boolean> = computed(
      () => name.value.trim() != "" && newMessage.value.trim() != ""
    );
    const initializeConnection = async () => {
      await connection.start();

      connection.on(
        "broadcastMessage",
        (id: string, name: string, message: string) => {
          messages.value = [...messages.value, { id, name, text: message }];
        }
      );
    };

    onMounted(initializeConnection);

    return {
      connection,
      messages,
      name,
      newMessage,
      isNewMessageValid,
    };
  },
  methods: {
    async sendMessage() {
      if (this.name.trim() && this.newMessage.trim()) {
        await this.connection.send("send", this.name, this.newMessage);
        this.newMessage = "";
      }
    },
  },
  components: {},
});
</script>
