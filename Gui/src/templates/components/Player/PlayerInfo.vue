<!-- PlayerInfo: begin -->
<script setup lang='ts'>
import Logo from '@/assets/logo.svg';

defineProps({
  title: {
    type: String,
    default: 'Unknown',
  },
  artists: {
    type: String,
    default: 'Unknown',
  },
  artworkUrl: {
    type: String,
    default: 'logo',
  }
});
</script>

<template>
  <!-- PlayerInfo/template: begin -->
  <div class="flex items-center space-x-4">
    <!-- PlayerInfo/template/artwork: begin -->
    <div class="relative h-14 w-14 flex-shrink-0 rounded bg-white">
      <img class="absolute inset-0 h-full w-full rounded object-cover" :src="artworkUrl === 'logo' ? Logo : artworkUrl">
    </div>
    <!-- PlayerInfo/template/artwork: end -->

    <!-- PlayerInfo/template/details: begin -->
    <div class="space-y-1">
      <!-- Song title -->
      <p class="font-bold leading-tight max-w-40">
        <transition name="slide-up" mode="out-in">
          <Vue3Marquee class="overflow-hidden" :duration="15"
            gradient-length='4px' v-if="title && title.length > 8">
            <span v-text="title"></span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
          </Vue3Marquee>
          <span v-text="title" v-else></span>
        </transition>
      </p>

      <!-- Song artist(s) -->
      <p class="text-sm font-light leading-tight max-w-40">
        <transition name="slide-up" mode="out-in">
          <Vue3Marquee class="overflow-hidden" :duration="10"
            gradient-length='4px' v-if="artists && artists.length > 6">
            <span v-text="artists"></span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
          </Vue3Marquee>
          <span v-text="artists" v-else></span>
        </transition>
      </p>
    </div>
    <!-- PlayerInfo/template/details: end -->
  </div>
  <!-- PlayerInfo/template: end -->
</template>

<style scoped>
.slide-up-enter-active,
.slide-up-leave-active {
  transition: all 0.25s ease;
}

.slide-up-enter-from {
  opacity: 0;
  transform: translateY(30px);
}

.slide-up-leave-to {
  opacity: 0;
  transform: translateY(-30px);
}
</style>
<!-- PlayerInfo: end -->