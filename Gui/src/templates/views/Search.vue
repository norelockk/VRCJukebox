<script setup lang='ts'>
import { ref } from 'vue';
import { isEmpty, isNull } from 'lodash';

import { api, socket } from '@/core';
import { usePlaylistStore } from '@/store';
import { GlobalSearchResponse, SpotifySearchResponse, WebSocketSend, YoutubeSearchResponse, PlaylistResponse } from '@/types';

const query = ref<string>('');
const loading = ref<boolean>(false);
const results = ref<YoutubeSearchResponse[] | SpotifySearchResponse[]>([]);
const platform = ref<string>('spotify');
const downloading = ref<{ [key: string]: boolean }>({});
const resultPlatform = ref<string>('');

const columns: { [key: string]: any[] } = {
  'spotify': [['Artists'], ['Title'], ['Actions']],
  'youtube': [['Channel'], ['Name'], ['Actions']],
}

const changePlatform = (): void => {
  platform.value = platform.value === 'spotify' ? 'youtube' : 'spotify';
};

const search = async (): Promise<void> => {
  if (loading.value) return;

  if (isEmpty(query.value) || isNull(query.value)) return;
  if (!loading.value) {
    loading.value = true;
    if (!isEmpty(results.value)) results.value = [];
  }

  const response = await api.getWithParams<GlobalSearchResponse>('search', { query: query.value, platform: platform.value });

  if (response && response.platform === platform.value && loading.value) {
    loading.value = false;
    results.value = response.results;
    resultPlatform.value = response.platform;

    console.log(response);
  }
};

const playlist = usePlaylistStore();

function updateDownload(data: any) {
  let done: boolean = false;

  switch (data.State) {
    case 'Downloading': {
      downloading.value[data.Id] = true;
      break;
    }
    case 'Already_Downloaded':
    case 'Downloaded': {
      playlist.update();
      delete downloading.value[data.Id];
      break;
    }
  }

  if (Object.entries(downloading.value).length === 0)
    done = true;
  
  done && socket.off('Download', updateDownload);
}

const download = (id: string): void => {
  if (id in downloading.value)
    return;

  downloading.value[id] = false;

  const data: WebSocketSend = {
    Data: {
      Id: id,
      Platform: platform.value,
    },
    Event: "Download",
  };

  socket.on('Download', updateDownload);
  socket.send(JSON.stringify(data));
};

const isTrackAlreadyDownloaded = (author: string, title: string): boolean => {
  const song = playlist.list.find((s: PlaylistResponse) => s.Artist === author && s.Title === title && !!s.IsDownloaded);
  if (song)
    return true;

  return false;
}
</script>

<template>
  <div class="relative">
    <label for="Search" class="sr-only">Search</label>

    <form @submit.prevent="search">
      <input type="text" autocomplete="off" v-model="query" id="Search" placeholder="Search for..."
        class="px-3 w-full rounded-md dark:bg-neutral-700 bg-gray-100 outline-0 py-2.5 pe-10 sm:text-sm" />

      <span class="absolute inset-y-0 end-0 grid w-10 place-content-center pr-4">
        <div class="flex">
          <button @click="search" type="submit" class="transition-all dark:text-white text-gray-600 dark:hover:text-neutral-200 hover:text-gray-700">
            <span class="sr-only">Search</span>

            <i class="fa-solid fa-magnifying-glass"></i>
          </button>

          <button @click="changePlatform" type="button" class="dark:text-white text-gray-600 transition-all px-2"
            :class="platform === 'spotify' ? 'hover:text-green-500' : 'hover:text-red-500'"
            v-tippy="{ content: 'Click here to change search platform' }">
            <span class="sr-only">Platform</span>

            <i class="fa-brands" :class="'fa-' + platform"></i>
          </button>
        </div>
      </span>
    </form>
  </div>

  <div class="relative py-2" v-if="isEmpty(results) || loading || platform !== resultPlatform">
    <!-- Show loading or no results message here -->
    <div v-if="isEmpty(results) && loading || !isEmpty(results) && loading">
      <div class="loader"></div>
    </div>

    <div v-if="(!isEmpty(results) || !loading) || !isEmpty(results)">
      <span>Search for something in {{ platform }}</span>
    </div>
  </div>

  <div class="relative overflow-y-auto max-h-96 py-2 scrollbar-custom" v-else>
    <table class="w-full text-sm text-left rtl:text-right dark:text-white text-gray-500">
      <thead class="text-xs rounded-md dark:text-white text-gray-700 uppercase">
        <tr>
          <th v-for="column in columns[platform]" :key="column[0]" v-text="column[0]" class="py-3 px-6" scope="col"></th>
        </tr>
      </thead>
      <tbody>
        <!-- YouTube -->
        <tr
          :data-id="(result as YoutubeSearchResponse).Id.Value" class="dark:bg-neutral-900 bg-white dark:border-neutral-800 border-b transition-all hover:dark:bg-neutral-700 hover:bg-gray-200"
          v-if="platform === 'youtube'"
          v-for="result in results" :key="(result as YoutubeSearchResponse).Id.Value"
          scope="row"
        >
          <a :href="(result as YoutubeSearchResponse).Author.ChannelUrl" target="_blank">
            <td class="py-3 px-6" v-text="(result as YoutubeSearchResponse).Author.Title"></td>
          </a>
          <td class="py-3 px-6" v-text="(result as YoutubeSearchResponse).Title"></td>
          <td class="py-3 px-6">
            <button
              v-tippy="{ content: 'Download' }"
              @click="download((result as YoutubeSearchResponse).Id.Value)"
              :disabled="downloading[(result as YoutubeSearchResponse).Id.Value] || isTrackAlreadyDownloaded((result as YoutubeSearchResponse).Author.ChannelTitle, (result as YoutubeSearchResponse).Title)"
              :class="isTrackAlreadyDownloaded((result as YoutubeSearchResponse).Author.ChannelTitle, (result as YoutubeSearchResponse).Title) ? 'bg-green-600' : !downloading[(result as YoutubeSearchResponse).Id.Value] ? 'bg-accent' : 'bg-gray-600'"
              class="hover:bg-gray-600 transition-all h-5 text-white rounded-sm w-5"
            >
              <i v-if="downloading[(result as YoutubeSearchResponse).Id.Value]" class="fa-solid fa-spinner fa-spin"></i>
              <i v-else-if="isTrackAlreadyDownloaded((result as YoutubeSearchResponse).Author.ChannelTitle, (result as YoutubeSearchResponse).Title)" class="fa-solid fa-check"></i>
              <i v-else class="fa-solid fa-download"></i>
            </button>
          </td>
        </tr>

        <!-- Spotify -->
        <tr
          :data-id="(result as SpotifySearchResponse).Id.Value"
          class="dark:bg-neutral-900 bg-white dark:border-neutral-800 border-b transition-all hover:dark:bg-neutral-700 hover:bg-gray-200"
          v-else
          v-for="result in results"
          :key="(result as SpotifySearchResponse).Id.Value"
          scope="row"
        >
          <td class="py-3 px-6" v-text="(result as SpotifySearchResponse).Artists.map(a => a.Name).join(', ')"></td>
          <td class="py-3 px-6" v-text="(result as SpotifySearchResponse).Title"></td>
          <td class="py-3 px-6">
            <button
              v-tippy="{ content: 'Download' }"
              @click="download((result as SpotifySearchResponse).Id.Value)"
              :disabled="downloading[(result as SpotifySearchResponse).Id.Value] || isTrackAlreadyDownloaded((result as SpotifySearchResponse).Artists.map(a => a.Name).join(', '), (result as SpotifySearchResponse).Title)"
              :class="isTrackAlreadyDownloaded((result as SpotifySearchResponse).Artists.map(a => a.Name).join(', '), (result as SpotifySearchResponse).Title) ? 'bg-green-600' : !downloading[(result as SpotifySearchResponse).Id.Value] ? 'bg-accent' : 'bg-gray-600'"
              class="hover:bg-gray-600 transition-all h-5 text-white rounded-sm w-5"
            >
              <i v-if="downloading[(result as SpotifySearchResponse).Id.Value]" class="fa-solid fa-spinner fa-spin"></i>
              <i v-else-if="isTrackAlreadyDownloaded((result as SpotifySearchResponse).Artists.map(a => a.Name).join(', '), (result as SpotifySearchResponse).Title)" class="fa-solid fa-check"></i>
              <i v-else class="fa-solid fa-download"></i>
            </button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<style scoped>
.loader {
  width: 45px;
  aspect-ratio: 1;
  --c: no-repeat linear-gradient(#000 0 0);
  background:
    var(--c) 0% 100%,
    var(--c) 50% 100%,
    var(--c) 100% 100%;
  animation: l2 1s infinite linear;
}

@keyframes l2 {
  0% {
    background-size: 20% 100%, 20% 100%, 20% 100%
  }

  20% {
    background-size: 20% 60%, 20% 100%, 20% 100%
  }

  40% {
    background-size: 20% 80%, 20% 60%, 20% 100%
  }

  60% {
    background-size: 20% 100%, 20% 80%, 20% 60%
  }

  80% {
    background-size: 20% 100%, 20% 100%, 20% 80%
  }

  100% {
    background-size: 20% 100%, 20% 100%, 20% 100%
  }
}
</style>