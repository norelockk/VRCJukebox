import { StatePlayback } from "@/types/enums";

interface StateProgress {
  Length: number;
  Percent: number;
  Position: number;
}

interface StateInfo {
  Muted: boolean;
  Volume: number;
  Playback: StatePlayback;
}

interface StateTrack {
  Id: string;
  Title: string;
  Author: string;
}

export interface StateResponse {
  State: StateInfo;
  Track: StateTrack;
  Progress: StateProgress;
}

export interface SeekResponse {
  Length: number;
  Volume: number;
  Position: number;
}