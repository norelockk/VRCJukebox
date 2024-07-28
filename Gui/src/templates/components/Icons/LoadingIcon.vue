<template>
  <div class="icon"></div>
</template>

<style scoped>
.icon {
  width: 45px;
  aspect-ratio: 1;
  background:
    no-repeat linear-gradient(var(--icon-color) 0 0) 0% 100%,
    no-repeat linear-gradient(var(--icon-color) 0 0) 50% 100%,
    no-repeat linear-gradient(var(--icon-color) 0 0) 100% 100%;
  animation: l2 1s infinite ease;
}

@keyframes l2 {
  0% {
    background-size: 20% 100%, 20% 100%, 20% 100%;
  }

  20% {
    background-size: 20% 60%, 20% 100%, 20% 100%;
  }

  40% {
    background-size: 20% 80%, 20% 60%, 20% 100%;
  }

  60% {
    background-size: 20% 100%, 20% 80%, 20% 60%;
  }

  80% {
    background-size: 20% 100%, 20% 100%, 20% 80%;
  }

  100% {
    background-size: 20% 100%, 20% 100%, 20% 100%;
  }
}
</style>

<script setup lang='ts'>
import { useDark } from '@vueuse/core';
import { onMounted, watchEffect } from 'vue';

const isDark = useDark();

onMounted(() => {
  document.documentElement.style.setProperty('--icon-color', isDark.value ? '#fff' : '#000');
  isDark.value ? document.documentElement.classList.add('dark') : document.documentElement.classList.remove('dark');
});

watchEffect(() => {
  document.documentElement.style.setProperty('--icon-color', isDark.value ? '#fff' : '#000');
});
</script>