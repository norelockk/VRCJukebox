(window as any).DEV_MODE = process.env.NODE_ENV === 'development';

import './assets/index.css';
import 'tippy.js/dist/tippy.css';

import { createPinia } from 'pinia';
import { createApp, defineAsyncComponent } from 'vue';
import VueTippy from 'vue-tippy';
import Vue3Marquee from 'vue3-marquee';

import createGuiRouter from './router';
import { useLocalization } from './store';

const App = defineAsyncComponent(() => import('@/templates/App.vue'));

const app = createApp(App);
const pinia = createPinia();

const importLocales = import.meta.glob("@/locales/*.json", { eager: true });

async function bootstrap() {
  app.use(pinia);
  app.use(
    VueTippy,
    {
      directive: 'tippy', // => v-tippy
      defaultProps: {
        placement: 'auto-end',
        allowHTML: true,
      }
    }
  )
  app.use(Vue3Marquee);
  createGuiRouter(app);

  const locales = await Promise.all(Object.entries(importLocales)
    .filter(([k]) => k.endsWith('.json'))
    .map(([k, v]) => [k.match(/\w+(?=\.json$)/)![0], v] as const)
    .map(([k, v]) => {
      const data = (v as any).default;

      return {
        code: k,
        name: data.name,
        data: data.strings,
        rtl: data.rtl
      };
    }));

  const locale = useLocalization();
  locale.init(locales);

  app.mount('#app');
}

bootstrap().catch(console.error);