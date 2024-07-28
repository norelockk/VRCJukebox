<!-- Wrapper: begin -->
<template>
  <div class="wrapper pl-64">
    <div class="wrap">
      <slot></slot>
    </div>
  </div>
</template>

<style scoped>
.wrapper {
  @apply absolute w-full min-h-full;
}

.wrapper .wrap {
  @apply p-6 mb-20;
}
</style>
<!-- Wrapper: end -->