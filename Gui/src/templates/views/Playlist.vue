<script setup lang='ts'>
import { isEmpty } from 'lodash';
import { onMounted } from 'vue';
import { usePlaylistStore } from '@/store';
import { PlaylistResponse } from '@/types';

const columns = ['TABLE_PLAYLIST_ARTIST', 'TABLE_PLAYLIST_TITLE', 'TABLE_PLAYLIST_ACTIONS'];
const playlist = usePlaylistStore();

async function mounted(): Promise<void> {
  if (!isEmpty(playlist.list))
    await playlist.update();
}

onMounted(mounted);
</script>

<template>
  <div class="relative overflow-y-auto max-h-full">
    <table class="w-full text-sm text-left rtl:text-right dark:text-white text-gray-500">
      <thead class="text-xs rounded-md dark:text-white text-gray-700 uppercase">
        <tr>
          <th v-for="column in columns" :key="column" v-text="column" class="py-3 px-6" scope="col"></th>
        </tr>
      </thead>
      <tbody>
        <tr
          :data-id="(track as PlaylistResponse).Id" class="dark:bg-neutral-900 bg-white dark:border-neutral-800 border-b transition-all hover:dark:bg-neutral-700 hover:bg-gray-200"
          v-for="track in playlist.list" :key="(track as PlaylistResponse).Id"
          scope="row"
        >
          <td class="py-3 px-6" v-text="track.Artist"></td>
          <td class="py-3 px-6" v-text="track.Title"></td>
          <td class="py-3 px-6">
            <!-- <button
              v-tippy="{ content: 'Download' }"
              @click="download((result as YoutubeSearchResponse).Id.Value)"
              :disabled="downloading[(result as YoutubeSearchResponse).Id.Value]"
              :class="!downloading[(result as YoutubeSearchResponse).Id.Value] ? 'bg-accent' : 'bg-gray-600'"
              class="hover:bg-gray-600 transition-all h-5 text-white rounded-sm w-5"
            >
              <i v-if="downloading[(result as YoutubeSearchResponse).Id.Value]" class="fa-solid fa-spinner fa-spin"></i>
              <i v-else class="fa-solid fa-download"></i>
            </button> -->
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>