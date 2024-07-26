// Utils types
export type EventHandler<T = any> = (eventData: T) => void;

// API communication types
export type RequestOptions = RequestInit & { timeout?: number };
export type RequestInterceptor = (options: RequestOptions) => RequestOptions | Promise<RequestOptions>;
export type ResponseInterceptor = <T>(response: T) => T | Promise<T>;

// All enums
export * from './enums';

// All interfaces
export * from './interfaces';
