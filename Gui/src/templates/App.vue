<!-- App: begin -->
<script setup lang='ts'>
import { defineAsyncComponent } from 'vue';

import Loading from './components/Loading.vue';
import Sidebar from './components/Sidebar/Sidebar.vue';
import Container from './components/Container.vue';

const Player = defineAsyncComponent(() => import('./components/Player/Player.vue'))
</script>

<template>
  <!-- App/template: begin -->
  <Loading />

  <div class="leading-normal tracking-normal">
    <Container>
      <Sidebar />
      <div class="absolute w-full pl-0 pl-64 min-h-full">
        <div class="p-6 mb-20">
          <!-- <transition name="fade" mode="out-in"> -->
          <router-view />
          <!-- </transition> -->
        </div>
      </div>
      <Player />
    </Container>
  </div>
  <!-- App/template: end -->
</template>

<style scoped>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.5s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
<!-- App: end -->