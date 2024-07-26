// Spotify search
interface SpotifyIdResponse {
  Value: string;
}

interface SpotifyArtistsResponse {
  Id: SpotifyIdResponse;
  Name: string;
}

interface SpotifyImagesResponse {
  Url: string;
  Width: number;
  Height: number;
}

interface SpotifyAlbumResponse {
  Id: SpotifyIdResponse;
  Url: string;
  Name: string;
  Images: SpotifyImagesResponse[];
  Artists: SpotifyArtistsResponse[];
  TotalTracks: number;
}

export interface SpotifySearchResponse {
  Id: SpotifyIdResponse;
  Url: string;
  Title: string;
  Album: SpotifyAlbumResponse;
  Artists: SpotifyArtistsResponse[];
  Duration: number;
  Explicit: boolean;
  PreviewUrl: string;
}

// YT search
interface YoutubeIdResponse {
  Value: string;
}

interface YoutubeAuthorResponse {
  Title: string;
  ChannelId: YoutubeIdResponse;
  ChannelUrl: string;
  ChannelTitle: string;
}

interface YoutubeResolutionResponse {
  Width: number;
  Height: number;
}

interface YoutubeThumbnailResponse {
  Url: string;
  Resolution: YoutubeResolutionResponse;
}

export interface YoutubeSearchResponse {
  Id: YoutubeIdResponse;
  Url: string;
  Title: string;
  Author: YoutubeAuthorResponse;
  Duration: string;
  Thumbnails: YoutubeThumbnailResponse[]
}

// Response for request /api/search?query=x&platform=x
export interface GlobalSearchResponse {
  query: string;
  platform: string;
  results: YoutubeSearchResponse[] | SpotifySearchResponse[];
}