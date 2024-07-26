// export const API_URL: string = process.env.NODE_ENV === 'development' ? `${location.protocol}//localhost:5000/api` : '/api';

import { isStringNumericHostname } from '@/utils';

export default class Constants {
  public static readonly API_URL: string = process.env.NODE_ENV === 'development'
    ? `${location.protocol}//localhost:5000/api`
    : `${location.protocol}//${isStringNumericHostname(location.hostname) || location.hostname === 'localhost' ? location.hostname + ':' + location.port : location.hostname}/api`;
  public static readonly APP_NAME: string = 'VRCJukebox';
}