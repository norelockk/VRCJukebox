import Constants from '@/constants';

import { EventEmitter } from './helpers';
import { ApiCommunication, WebSocketCommunication } from './network';

export const api: ApiCommunication = new ApiCommunication(Constants.API_URL);
export const events: EventEmitter = new EventEmitter();
export const socket: WebSocketCommunication = new WebSocketCommunication();