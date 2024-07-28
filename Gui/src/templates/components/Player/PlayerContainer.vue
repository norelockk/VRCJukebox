<!-- PlayerContainer: begin -->
<template>
  <!-- PlayerContainer/template: begin -->
  <div class="container player-inside">
    <slot></slot>
  </div>
  <!-- PlayerContainer/template: end -->
</template>

<style scoped>
@media (min-width: 768px) {
  .container {
    max-width: 48rem;
  }
}

@media (min-width: 640px) {
  .container {
    margin-left: auto;
    margin-right: auto;
    max-width: 42rem;
    padding-left: 0px;
    padding-right: 0px;
  }
}

.container {
  width: 100%;
  padding-left: 1rem;
  padding-right: 1rem;
}

.player-inside {
  @apply relative flex items-center justify-between mt-2;
}
</style>
<!-- PlayerContainer: end -->