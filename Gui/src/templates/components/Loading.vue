<!-- Loading: begin -->
<script setup lang='ts'>
import { useLoadingStore } from '@/store';
const loading = useLoadingStore();
</script>

<template>
  <!-- Loading/template: begin -->
  <Transition name='fade'>
    <div class='loading-overlay' v-if='loading.showing'>
      <div class='loader'></div>
    </div>
  </Transition>
  <!-- Loading/template: end -->
</template>

<style scoped lang='scss'>
.loading-overlay {
  top: 0;
  left: 0;
  bottom: 0;
  width: 100%;
  height: 100%;
  // z-index: calc(var(--z-status) + 1);
  z-index: 999999999;
  display: flex;
  position: fixed;
  isolation: isolate;
  transition: 250ms ease;
  align-items: center;
  // pointer-events: none;
  flex-direction: column;
  justify-content: center;
  transition-property: left, width;
  backdrop-filter: grayscale(100%);

  &:after {
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    content: '';
    opacity: 0.65;
    z-index: -1;
    position: absolute;
    backdrop-filter: blur(32px);
    background-color: #000;
  }
}

.fade-enter-active,
.fade-leave-active {
  transition: all 250ms ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

/* HTML: <div class="loader"></div> */
.loader {
  width: 45px;
  aspect-ratio: 1;
  --c: no-repeat linear-gradient(#fff 0 0);
  background:
    var(--c) 0% 100%,
    var(--c) 50% 100%,
    var(--c) 100% 100%;
  animation: l2 1s infinite ease;
}

@keyframes l2 {
  0% {
    background-size: 20% 100%, 20% 100%, 20% 100%
  }

  20% {
    background-size: 20% 60%, 20% 100%, 20% 100%
  }

  40% {
    background-size: 20% 80%, 20% 60%, 20% 100%
  }

  60% {
    background-size: 20% 100%, 20% 80%, 20% 60%
  }

  80% {
    background-size: 20% 100%, 20% 100%, 20% 80%
  }

  100% {
    background-size: 20% 100%, 20% 100%, 20% 100%
  }
}
</style>
<!-- Loading: end -->