<!-- Sidebar: begin -->
<script setup lang='ts'>
import SidebarLogo from './SidebarLogo.vue';
import SidebarItem from './SidebarItem.vue';
import SidebarContent from './SidebarContent.vue';
import SidebarWrapper from './SidebarWrapper.vue';
import SidebarCategory from './SidebarCategory.vue';
</script>

<template>
  <!-- Sidebar/template: begin -->
  <SidebarWrapper>
    <SidebarLogo />

    <SidebarContent>
      <SidebarCategory str="SIDEBAR_NAVIGATION" />

      <SidebarItem
        v-for="route in $router.getRoutes()"
        :key="route.path"
        :to="route.path"
        :icon="(route.meta?.navigation as any).icon"  
        :str-title="(route.meta?.navigation as any).strings.title"
        :str-description="(route.meta?.navigation as any).strings.description"
      />
    </SidebarContent>
  </SidebarWrapper>
  <!-- Sidebar/template: end -->
</template>
<!-- Sidebar: end -->