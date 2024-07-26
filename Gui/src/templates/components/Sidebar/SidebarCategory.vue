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
  <p class="pl-4 dark:text-white text-sm font-light mb-3" v-text="t(str)"></p>
  <!-- SidebarCategory/template: end -->
</template>
<!-- SidebarCategory: end -->