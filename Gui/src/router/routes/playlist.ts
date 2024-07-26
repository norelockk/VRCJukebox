import { RouteRecordRaw } from 'vue-router';

const home: RouteRecordRaw = {
  path: '/',
  meta: {
    navigation: {
      icon: 'fa-solid fa-music',
      strings: {
        title: 'PLAYLIST',
        description: 'PLAYLIST_DESCRIPTION'
      }
    },
    // middleware: 'protected',
    // requiredAuth: true,
  },
  component: () => import('@/templates/views/Playlist.vue')
};

export default home;