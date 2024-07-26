export interface ILoadingProgress {
  value: number;
  total: number;
  enabled: boolean;
  indeterminate: boolean;
}

export interface ILoadingState {
  text: string;
  show: boolean;
  delayMS: number;
  progress: ILoadingProgress;
}

export interface ILoadingStart {
  name: string;
}

export interface ILoadingFinish {
  name: string;
  wait?: boolean;
}