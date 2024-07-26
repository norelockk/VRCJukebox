<!-- PlayerProgress.vue: begin -->
<script setup lang='ts'>
import { socket } from '@/core';
import { WebSocketSend } from '@/types';
import { defineProps, onMounted, ref } from 'vue';

const props = defineProps<{
  progress: number;
  length: number;
}>();

const progressBar = ref<HTMLDivElement | null>(null);

const seek = (event: MouseEvent) => {
  if (!progressBar.value) return;

  const rect = progressBar.value.getBoundingClientRect();
  const clickPosition = event.clientX - rect.left;
  const newProgress = clickPosition / rect.width;

  const newPosition = newProgress * props.length;

  const data: WebSocketSend = {
    Event: 'Seek',
    Data: {
      Value: newPosition,
      Action: 'Position',
    },
  };

  socket.send(JSON.stringify(data));
};

onMounted(() => {
  if (progressBar.value) progressBar.value.addEventListener('click', seek);
});
</script>

<template>
  <!-- PlayerProgress/template: begin -->
  <div ref="progressBar" class="absolute transition-all top-0 w-full h-1 hover:h-3 hover:-translate-y-2 cursor-pointer dark:bg-neutral-700 bg-white">
    <div class="absolute h-full transition-all dark:bg-white bg-primary" :style="{ width: `${props.progress * 100}%` }"></div>
  </div>
  <!-- PlayerProgress/template: end -->
</template>

<style scoped>
</style>
<!-- PlayerProgress.vue: end -->
