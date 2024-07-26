import { EventHandler } from '@/types';

export class EventEmitter<T = any> {
  private events: Map<string, EventHandler<T>[]> = new Map();

  public on(event: string, listener: EventHandler<T>): void {
    if (!this.events.has(event)) this.events.set(event, []);

    this.events.get(event)!.push(listener);
  }

  public off(event: string, listener: EventHandler<T>): void {
    if (!this.events.has(event)) return;

    const listeners = this.events.get(event)!;
    this.events.set(event, listeners.filter(l => l !== listener));
  }

  public emit(event: string, eventData?: T): void {
    if (!this.events.has(event)) return;

    const listeners = this.events.get(event)!;
    for (const listener of listeners) listener(eventData!);
  }

  public once(event: string, listener: EventHandler<T>): void {
    const onceListener: EventHandler<T> = (eventData: T) => {
      listener(eventData);
      this.off(event, onceListener);
    };
    
    this.on(event, onceListener);
  }
}