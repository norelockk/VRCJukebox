// https://github.com/KIRAKIRA-DOUGA/KIRAKIRA-Cerasus/blob/develop/utils/delay.ts

import { onMounted, onUnmounted, Ref, ref } from 'vue';

export const delay = (ms: number): Promise<void> => new Promise(resolve => setTimeout(resolve, ms));

const isb = new Int32Array(typeof SharedArrayBuffer !== 'undefined' ? new SharedArrayBuffer(4) : 1 as unknown as ArrayBufferLike);
export function sleep(ms: number): void {
	Atomics.wait(isb, 0, 0, ms);
}

export function createInterval(callback: () => void, ms: number): Ref<NodeJS.Timeout | undefined> {
	const timeoutId = ref<NodeJS.Timeout>();
	onMounted(() => {
		timeoutId.value = setInterval(callback, ms);
	});
	onUnmounted(() => {
		clearInterval(timeoutId.value);
	});
	return timeoutId;
}