<!-- PlayerControls: begin -->
<script setup lang='ts'>
import { ref, watch, defineProps } from 'vue';
import { socket } from '@/core';
import { StatePlayback, WebSocketSend } from '@/types';

const props = defineProps({
  state: {
    type: Number,
    default: StatePlayback.Stopped,
    required: false
  },
  volume: {
    type: Number,
    default: 1,
    required: false
  }
});

const state = ref<StatePlayback | number>(props.state);
const volume = ref<number>(props.volume * 100);
const lastVolume = ref<number>(volume.value);
const isDragging = ref<boolean>(false);
const threshold: number = 0.0012;

watch(
  () => [props.state, props.volume],
  ([newState, newVolume]) => {
    state.value = newState;
    if (!isDragging.value) {
      volume.value = newVolume * 100;
    }
  }
);

const playbackIcon = (state: StatePlayback): string => {
  return state === StatePlayback.Playing ? 'fa-pause' : 'fa-play';
};

const changeVolume = (event: Event): void => {
  const target = event.target as HTMLInputElement;
  const newVolume = Number(target.value);
  if (Math.abs(newVolume - lastVolume.value) > threshold) {
    volume.value = newVolume;
  }
};

const startDragging = (): void => {
  isDragging.value = true;
};

const stopDragging = (): void => {
  if (!isDragging.value) return;
  isDragging.value = false;

  if (volume.value !== lastVolume.value) {
    const data: WebSocketSend = {
      Event: 'Seek',
      Data: {
        Value: volume.value / 100,
        Action: 'Volume',
      }
    };
    socket.send(JSON.stringify(data));
    lastVolume.value = volume.value;
  }
};

const updatePlaybackState = (data: { Playing: StatePlayback }): void => {
  if (state.value !== data.Playing) state.value = data.Playing;
  socket.off('Toggle', updatePlaybackState);
};

const changePlaybackState = (): void => {
  const data: WebSocketSend = {
    Event: 'Toggle',
    Data: { Action: 'Play' }
  };
  socket.on('Toggle', updatePlaybackState);
  socket.send(JSON.stringify(data));
};
</script>

<template>
  <!-- PlayerControls/template: begin -->
  <!-- Button (play/pause etc) -->
  <div class="inset-center hidden md:block">
    <button @click="changePlaybackState" class="flex h-11 w-11 cursor-pointer transition-all dark:bg-neutral-700 hover:dark:bg-neutral-600 bg-gray-200 hover:bg-gray-400 items-center justify-center rounded-full">
      <i class="dark:text-white text-black mx-2 fa-solid" :class="playbackIcon(state)"></i>
    </button>
  </div>

  <!-- Volume control -->
  <div class="flex items-center space-x-6">
    <div class="hidden items-center space-x-3 md:flex">
      <i class="fa-solid fa-volume-high"></i>
      <input
        type="range"
        @input="changeVolume"
        @mousedown="startDragging"
        @mouseup="stopDragging"
        @touchstart="startDragging"
        @touchend="stopDragging"
        :value="volume"
        min="0"
        max="100"
        class="volume w-full h-2 rounded-lg appearance-none cursor-pointer"
      />
    </div>
  </div>
  <!-- PlayerControls/template: end -->
</template>

<style scoped>
.volume {
  @apply w-full h-2 rounded-lg cursor-pointer;
  -webkit-appearance: none;
  appearance: none;
}

.volume::-webkit-slider-runnable-track {
  @apply dark:bg-neutral-700 bg-gray-200 rounded;
  height: 8px;
}

.volume::-moz-range-track {
  @apply dark:bg-neutral-700 bg-gray-200 rounded;
  height: 8px;
}

.volume::-ms-track {
  @apply bg-transparent;
  height: 8px;
}

.volume::-webkit-slider-thumb {
  @apply w-4 h-4 bg-white rounded-full cursor-pointer;
  -webkit-appearance: none;
  margin-top: -4px;
}

.volume::-moz-range-thumb {
  @apply w-4 h-4 bg-white rounded-full cursor-pointer;
}

.volume::-ms-thumb {
  @apply w-4 h-4 bg-white rounded-full cursor-pointer;
}
</style>
<!-- PlayerControls: end -->
