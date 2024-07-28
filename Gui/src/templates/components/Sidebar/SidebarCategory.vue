<!-- SidebarCategory: begin -->
<script setup lang='ts'>
import { useLocalization } from '@/store';

const { t } = useLocalization();

defineProps({
  str: {
    type: String,
    required: true
  }
});
</script>

<template>
  <!-- SidebarCategory/template: begin -->
  <p class="sidebar-category" v-text="t(str)"></p>
  <!-- SidebarCategory/template: end -->
</template>

<style scoped>
.sidebar-category {
  @apply pl-4 text-accent dark:text-white text-sm font-light mb-3;
}
</style>
<!-- SidebarCategory: end -->