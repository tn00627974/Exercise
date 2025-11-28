export interface WeatherData {
  temp: string;
  condition: string;
  humidity: string;
  wind: string;
  location: string;
}

export interface GroundingChunk {
  web?: {
    uri: string;
    title: string;
  };
}

export interface SearchResult {
  rawText: string;
  parsedData: WeatherData | null;
  groundingChunks: GroundingChunk[];
}

export interface SearchResult {
  rawText: string;
  parsedData: WeatherData | null;
  groundingChunks: GroundingChunk[];
}