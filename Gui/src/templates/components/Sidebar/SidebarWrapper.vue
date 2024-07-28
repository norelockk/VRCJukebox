<!-- SidebarWrapper: begin -->
<template>
  <!-- SidebarWrapper/template: begin -->
  <div class="sidebar">
    <slot></slot>
  </div>
  <!-- SidebarWrapper/template: end -->
</template>

<style scoped>
.sidebar {
  @apply w-64 fixed md:top-0 md:left-0 transition-all h-screen lg:block dark:text-white text-accent z-30;
}
</style>
<!-- SidebarWrapper: end -->