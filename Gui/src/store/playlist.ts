import { api } from '@/core';
import { defineStore } from 'pinia';
import { PlaylistResponse } from '@/types';
import { useInitializableStore } from './initializer';

export const usePlaylistStore = useInitializableStore(
  defineStore('playlist', {
    state: () => ({
      tbl: [] as PlaylistResponse[],
    }),

    getters: {
      list: state => state.tbl
    },

    actions: {
      async initialize() {
        await this.update();
      },

      async update() {
        const response = await api.get<PlaylistResponse[]>('playlist');
        if (response) this.tbl = response;
      },
    }
  })
);