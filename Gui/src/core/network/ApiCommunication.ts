import { RequestInterceptor, RequestOptions, ResponseInterceptor } from '@/types';

export class ApiCommunication {
  private baseUrl: string;
  private defaultHeaders: HeadersInit;
  private requestInterceptors: RequestInterceptor[] = [];
  private responseInterceptors: ResponseInterceptor[] = [];

  constructor(baseUrl: string) {
    this.baseUrl = baseUrl;
    this.defaultHeaders = {
      'Content-Type': 'application/json',
    };
  }

  private async request<T>(url: string, options: RequestOptions): Promise<T> {
    try {
      options = await this.applyRequestInterceptors(options);
      const response = await this.fetchWithTimeout(url, options);

      if (!response.ok)
        throw new Error(`HTTP error status: ${response.status}`);

      let data: T = await response.json();
      data = await this.applyResponseInterceptors(data);
      return data;
    } catch (error) {
      console.error('API request error:', error);
      throw error;
    }
  }

  private async fetchWithTimeout(url: string, options: RequestOptions): Promise<Response> {
    const { timeout = 8000, ...fetchOptions } = options;
    fetchOptions.mode = 'cors';

    const controller = new AbortController();
    const id = setTimeout(() => controller.abort(), timeout);
    fetchOptions.signal = controller.signal;

    try {
      const response = await fetch(url, fetchOptions);
      clearTimeout(id);
      return response;
    } catch (error) {
      clearTimeout(id);
      throw error;
    }
  }

  private async applyRequestInterceptors(options: RequestOptions): Promise<RequestOptions> {
    for (const interceptor of this.requestInterceptors)
      options = await interceptor(options);

    return options;
  }

  private async applyResponseInterceptors<T>(response: T): Promise<T> {
    for (const interceptor of this.responseInterceptors)
      response = await interceptor(response);

    return response;
  }

  public get<T>(endpoint: string): Promise<T> {
    return this.request<T>(`${this.baseUrl}/${endpoint}`, {
      method: 'GET',
      headers: this.defaultHeaders,
    });
  }

  public put<T>(endpoint: string, body: any): Promise<T> {
    return this.request<T>(`${this.baseUrl}/${endpoint}`, {
      method: 'PUT',
      headers: this.defaultHeaders,
      body: JSON.stringify(body),
    });
  }

  public post<T>(endpoint: string, body: any): Promise<T> {
    return this.request<T>(`${this.baseUrl}/${endpoint}`, {
      method: 'POST',
      headers: this.defaultHeaders,
      body: JSON.stringify(body),
    });
  }

  public delete<T>(endpoint: string): Promise<T> {
    return this.request<T>(`${this.baseUrl}/${endpoint}`, {
      method: 'DELETE',
      headers: this.defaultHeaders,
    });
  }

  public getWithParams<T>(endpoint: string, params?: Record<string, any>): Promise<T> {
    const url = new URL(`${this.baseUrl}/${endpoint}`);
    if (params) Object.keys(params).forEach(key => url.searchParams.append(key, params[key]));

    return this.request<T>(url.toString(), {
      method: 'GET',
      headers: this.defaultHeaders,
    });
  }

  public addRequestInterceptor(interceptor: RequestInterceptor): void {
    this.requestInterceptors.push(interceptor);
  }

  public addResponseInterceptor(interceptor: ResponseInterceptor): void {
    this.responseInterceptors.push(interceptor);
  }
}