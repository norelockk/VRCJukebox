<!-- SidebarLogo: begin -->
<script setup lang='ts'>
import Logo from '@/assets/logo.svg';
import Constants from '@/constants';
</script>

<template>
  <!-- SidebarLogo/template: begin -->
  <div class="sidebar-logo">
    <div class="logo">
      <!-- SidebarLogo/template/logotype: begin -->
      <img :src="Logo" class="w-8 h-8" />
      <!-- SidebarLogo/template/logotype: end -->

      <!-- SidebarLogo/template/text: begin -->
      <p class="font-bold text-2xl pl-2" v-text="Constants.APP_NAME"></p>
      <!-- SidebarLogo/template/text: end -->
    </div>
  </div>
  <!-- SidebarLogo/template: end -->
</template>

<style scoped>
.sidebar-logo {
  @apply w-full h-20 border-b dark:border-neutral-500 flex px-2 items-center mb-5;
}

.sidebar-logo .logo {
  @apply dark:text-white text-accent px-2 flex pointer-events-none touch-none;
}
</style>
<!-- SidebarLogo: end -->