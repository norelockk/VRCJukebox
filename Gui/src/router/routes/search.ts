import { RouteRecordRaw } from 'vue-router';

const home: RouteRecordRaw = {
  path: '/search',
  meta: {
    navigation: {
      icon: 'fa-solid fa-magnifying-glass',
      strings: {
        title: 'SEARCH',
        description: 'SEARCH_DESCRIPTION'
      }
    },
    // middleware: 'protected',
    // requiredAuth: true,
  },
  component: () => import('@/templates/views/Search.vue')
};

export default home;