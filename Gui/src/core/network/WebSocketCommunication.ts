import { api } from '..';
import { EventEmitter } from '../helpers';
import { isStringNumericHostname, stringContains } from '@/utils';
import { ResolveWSSResponse, WebSocketHandshake, WebSocketMessage } from '@/types';
import { useLoadingStore } from '@/store';

export class WebSocketCommunication extends EventEmitter {
  private usable: boolean = false;
  private shouldReconnect: boolean = true;
  private readonly reconnectInterval: number = 1000;

  private url?: string | undefined = undefined;
  private websocket?: WebSocket | undefined = undefined;
  private handshakeToken?: string | undefined = undefined;

  // Make connection to WebSocket Communication
  private connect(): void {
    if (this.websocket && this.websocket.readyState === WebSocket.OPEN)
      throw new ReferenceError('WebSocket Communication is already initialized');

    this.websocket = new WebSocket(this.url!);

    this.websocket.onopen = async (): Promise<void> => {
      this.emit('open', this.url);
      await this.handshake();
    };

    this.websocket.onerror = error => this.emit('error', error);

    this.websocket.onclose = (): void => {
      this.emit('close', this.shouldReconnect);
      this.websocket = undefined;
      
      const loading = useLoadingStore();

      if (this.shouldReconnect) setTimeout(() => this.connect(), this.reconnectInterval);
      if (!loading.showing) loading.start({ name: 'Connect' });
    };

    this.websocket.onmessage = (event: MessageEvent) => this.handleMessage(event.data);
  }

  // Handle messages from WebSocket Communication Server
  private handleMessage(data: any): void {
    if (typeof data === 'string') {
      try {
        const message: WebSocketMessage = JSON.parse(data);
        // console.log('received', message);

        if (message.Event)
          this.emit(message.Event, message.Response);
        else
          this.emit('message', message);
      } catch (e: any) {
        this.emit('error', e);

        throw new TypeError(`Handling message failed: ${e.message}`);
      }
    }
  }

  // Construct WebSocket Communication Connection
  private async construct(): Promise<void> {
    const response = await api.get<ResolveWSSResponse>('resolve-wss');

    if (response) {
      const protocol = location.protocol === 'https:' ? 'wss:' : 'ws:';
      const isLocalhost = location.hostname === 'localhost' || location.hostname === '127.0.0.1';
      const useResponseUrl = stringContains(response.url, '0.0.0.0') || isLocalhost;

      this.url = `${protocol}//${useResponseUrl ? response.url : isStringNumericHostname(location.hostname) ? location.hostname + ':' + response.port : location.hostname}`;
      console.log('connect with url', JSON.stringify(this.url));

      this.connect();
    }
  }

  // Packets
  /**
   * @method WebSocketCommunication.handshake
   * @description Sends an handshake message to initialize connection
   */
  private async handshake(): Promise<void> {
    if (!this.websocket || this.websocket.readyState !== WebSocket.OPEN)
      throw new ReferenceError('WebSocket Communication is not initialized');

    // TODO get anti-bot token authentication to websocket
    this.handshakeToken = 'dev';

    const data: WebSocketHandshake = {
      Data: {
        Token: this.handshakeToken
      },
      Event: "Handshake"
    };

    this.websocket.send(JSON.stringify(data));
  }

  // Communication methods
  public send(data: any): void {
    if (this.websocket && this.websocket.readyState === WebSocket.OPEN && this.usable)
      return this.websocket.send(data);

    this.emit('error', new ReferenceError('WebSocket Communication is not initialized'));
  }

  // Disconnect from communication
  public close(): void {
    if (this.websocket && this.websocket.readyState === WebSocket.OPEN) {
      if (this.usable) this.usable = false;

      this.websocket.close();
      this.shouldReconnect = false;
    }
  }

  // WebSocket Communication constructor
  constructor() {
    super();

    this.on('Handshaked', () => {
      const loading = useLoadingStore();
      
      if (!this.usable) this.usable = true;
      if (loading.show) loading.finish({ name: 'Connect', wait: true });
    });
    this.construct();
  }
}
