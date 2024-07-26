<!-- SidebarItem: begin -->
<script setup lang='ts'>
import { useLocalization } from '@/store';

const { t } = useLocalization();

defineProps({
  to: {
    type: String,
    default: '/',
    required: false,
  },
  icon: {
    type: String,
    required: false,
  },
  strTitle: {
    type: String,
    required: true,
  },
  strDescription: {
    type: String,
    default: null,
    required: false,
  }
});
</script>

<template>
  <!-- SidebarItem/template: begin -->
  <router-link
    class="w-full flex items-center mb-2 transition-all dark:text-white text-gray-600 h-14 pl-4 hover:bg-accent hover:text-white rounded-lg cursor-pointer"
    :to="to"
    exact-active-class="bg-accent text-white"
  >
    <!-- SidebarItem/template/icon: begin -->
    <i class="h-4 w-4 fill-current mr-3" v-if="typeof icon === 'string'" :class="icon"></i>
    <!-- SidebarItem/template/icon: end -->
    
    <!-- SidebarItem/template/titles: begin -->
    <div class="flex flex-col items-start">
      <span class="font-semibold" v-text="t(strTitle)"></span>
      <span class="text-xs font-normal" v-if="typeof strDescription === 'string'" v-text="t(strDescription)"></span>
    </div>
    <!-- SidebarItem/template/titles: end -->
  </router-link>
  <!-- SidebarItem/template: end -->
</template>
<!-- SidebarItem: end -->