<!-- Player: begin -->
<script setup lang='ts'>
import PlayerInfo from './PlayerInfo.vue';
import PlayerProgress from './PlayerProgress.vue';
import PlayerControls from './PlayerControls.vue';
import PlayerContainer from './PlayerContainer.vue';

import Constants from '@/constants';

import { socket } from '@/core';
import { ref, onMounted } from 'vue';
import { StateResponse, SeekResponse, StatePlayback } from '@/types';

const title = ref<string | undefined>(undefined);
const length = ref<number>(0);
const volume = ref<number>(0);
const currentPlayingId = ref<string>('');
const artists = ref<string | undefined>(undefined);
const progress = ref<number>(0);
const playback = ref<StatePlayback | number>(StatePlayback.Stopped);

const shouldUpdateVolume = ref<boolean>(true);
const shouldUpdateProgress = ref<boolean>(true);

onMounted(() => {
  socket.on('Seek', (data: SeekResponse) => {
    volume.value = data.Volume;
    progress.value = data.Position / data.Length;

    shouldUpdateVolume.value = false;
    shouldUpdateProgress.value = false;
  });

  socket.on('State', (data: StateResponse) => {
    title.value = data.Track.Title;
    length.value = data.Progress.Length;
    currentPlayingId.value = data.Track.Id;

    if (shouldUpdateVolume.value) volume.value = data.State.Volume;
    if (shouldUpdateProgress.value) progress.value = data.Progress.Position / data.Progress.Length;

    artists.value = data.Track.Author;
    playback.value = data.State.Playback;

    shouldUpdateVolume.value = true;
    shouldUpdateProgress.value = true;
  });
});
</script>

<template>
  <!-- Player/template: begin -->
  <div class="player">
    <PlayerProgress :progress="progress" :length="length" />
    <PlayerContainer>
      <PlayerInfo :title="title" :artists="artists" :artwork-url="currentPlayingId.length !== 0 || currentPlayingId !== '-1' ? `${Constants.API_URL}/artwork?trackId=${currentPlayingId}` : 'logo'" />
      <PlayerControls :state="playback" :volume="volume" />
    </PlayerContainer>
  </div>
  <!-- Player/template: end -->
</template>

<style scoped>
.player {
  @apply fixed bottom-0 z-50 w-full py-3 dark:bg-neutral-800 bg-accent text-white
}
</style>
<!-- Player: end -->
