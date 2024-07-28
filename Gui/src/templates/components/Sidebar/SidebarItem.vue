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
    class="sidebar-item"
    :to="to"
    exact-active-class="active"
  >
    <!-- SidebarItem/template/icon: begin -->
    <i class="icon" v-if="typeof icon === 'string'" :class="icon"></i>
    <!-- SidebarItem/template/icon: end -->
    
    <!-- SidebarItem/template/titles: begin -->
    <div class="content">
      <span class="title" v-text="t(strTitle)"></span>
      <span class="description" v-if="typeof strDescription === 'string'" v-text="t(strDescription)"></span>
    </div>
    <!-- SidebarItem/template/titles: end -->
  </router-link>
  <!-- SidebarItem/template: end -->
</template>

<style scoped>
.sidebar-item {
  @apply w-full flex items-center mb-2 transition-all dark:text-white text-gray-600 h-14 pl-4 hover:bg-accent hover:text-white rounded-lg cursor-pointer;
}

.sidebar-item.active {
  @apply bg-accent text-white;
}

.sidebar-item .icon {
  @apply h-4 w-4 fill-current mr-3;
}

.sidebar-item .content {
  @apply flex flex-col items-start;
}

.sidebar-item .content .title {
  @apply font-semibold;
}

.sidebar-item .content .description {
  @apply text-xs font-normal;
} 
</style>
<!-- SidebarItem: end -->