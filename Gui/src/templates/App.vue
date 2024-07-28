<!-- App: begin -->
<script setup lang='ts'>
import { defineAsyncComponent } from 'vue';

import Loading from './components/Loading.vue';
import Wrapper from './components/Wrapper.vue';
import Sidebar from './components/Sidebar/Sidebar.vue';

const Player = defineAsyncComponent(() => import('./components/Player/Player.vue'));
</script>

<template>
  <!-- App/template: begin -->
  <Loading />

  <div class="jukebox-gui">
    <Sidebar />
    <Player />
    <Wrapper>
      <router-view />
    </Wrapper>
  </div>
  <!-- App/template: end -->
</template>

<style scoped>
.jukebox-gui {
  @apply flex relative h-full;
}
</style>
<!-- App: end -->