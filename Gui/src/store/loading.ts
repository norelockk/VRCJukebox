import { defineStore } from 'pinia';

import { useInitializableStore } from './initializer';
import { ILoadingFinish, ILoadingProgress, ILoadingStart, ILoadingState } from '@/types';

let queue: Record<string, boolean> = {
  'Connect': true,
};

export const useLoadingStore = useInitializableStore(
  defineStore('loading', {
    state: (): ILoadingState => ({
      text: 'Loading',
      show: true,
      delayMS: 250,
      progress: {
        value: 0,
        total: 0,
        enabled: true,
        indeterminate: true
      }
    }),

    getters: {
      delay: state => state.delayMS,
      showing: state => state.show,
      currentText: state => state.text,
      currentProgress: state => state.progress
    },

    actions: {
      setText(text: string) {
        if (typeof text !== 'string' || this.text === text)
          return;

        this.text = text;
      },

      setProgress(payload: ILoadingProgress) {
        this.progress.value = payload.value ?? 0;
        this.progress.total = payload.total ?? 0;
        this.progress.indeterminate = payload.indeterminate ?? true;
      },

      setProgressEnabled(enabled: boolean) {
        if (typeof enabled !== 'boolean' || this.progress.enabled === enabled)
          return;

        this.progress.enabled = enabled;
      },

      async start(payload: ILoadingStart) {
        return new Promise(resolve => {
          if (Object.keys(queue).length) {
            if (payload.name in queue) {
              console.warn(payload.name, 'is already queued');
              return;
            }
          } else {
            this.show = true;
          }

          queue[payload.name] = true;

          setTimeout(resolve, this.delay);
          console.info(payload.name, 'started');
        });
      },

      async finish(payload: ILoadingFinish) {
        return new Promise(resolve => {
          const name: string = payload.name ?? '';
          const waiting: boolean = payload.wait ?? false;

          const finished = (): void => {
            if (name in queue) delete queue[name];
            if (!Object.keys(queue).length) this.show = false;

            setTimeout(resolve, this.delay);
          };

          if (waiting)
            setTimeout(finished, this.delay);
          else
            finished();

          console.info(name, finished);
        });
      },

      async finishAll() {
        return new Promise(resolve => {
          if (Object(queue).length > 0)
            queue = {};

          const finished = (): void => {
            if (this.show) this.show = false;
            if (this.text.length > 0) this.text = '';

            setTimeout(resolve, this.delay);
          };

          finished();
        });
      },
    },
  })
);
