import { GoogleGenAI } from "@google/genai";
import { SearchResult, WeatherData, DailyForecast } from "../types";

const ai = new GoogleGenAI({ apiKey: process.env.API_KEY });

export const getWeatherData = async (location: string): Promise<SearchResult> => {
  const modelId = "gemini-2.5-flash"; // Supports Google Search Grounding

  // We ask for a specific format for both current weather and the 7-day forecast
  const prompt = `
    Find the current weather and 7-day forecast for ${location}.
    
    CRITICAL INSTRUCTION:
    1. The FIRST line of your response MUST be the current weather strictly in this format:
       RAW_DATA|{Temperature in Celsius}|{Condition}|{Humidity}|{Wind Speed}|{City Name}
    
    2. Immediately following that, provide exactly 7 lines for the 7-day forecast, each starting with "FORECAST|".
       Format: FORECAST|{Day Name (e.g. 週一)}|{Condition}|{Low Temp}|{High Temp}
       Example: FORECAST|週一|多雲|22|28

    After these data lines, provide a friendly, helpful weather report in Traditional Chinese (Taiwan usage).
    Include:
    1. A summary of how it feels.
    2. Clothing advice.
  `;

  try {
    const response = await ai.models.generateContent({
      model: modelId,
      contents: prompt,
      config: {
        tools: [{ googleSearch: {} }],
        // Note: responseMimeType and responseSchema are NOT allowed when using googleSearch
      },
    });

    const text = response.text || "";
    const groundingChunks = response.candidates?.[0]?.groundingMetadata?.groundingChunks || [];
    
    const lines = text.trim().split('\n');
    let parsedData: WeatherData | null = null;
    let tempParsed: Partial<WeatherData> = {};
    const forecast: DailyForecast[] = [];
    
    lines.forEach(line => {
      const cleanLine = line.trim();
      if (cleanLine.startsWith('RAW_DATA|')) {
        const parts = cleanLine.split('|');
        if (parts.length >= 6) {
          tempParsed = {
            temp: parts[1].trim(),
            condition: parts[2].trim(),
            humidity: parts[3].trim(),
            wind: parts[4].trim(),
            location: parts[5].trim(),
          };
        }
      } else if (cleanLine.startsWith('FORECAST|')) {
        const parts = cleanLine.split('|');
        if (parts.length >= 5) {
          forecast.push({
            day: parts[1].trim(),
            condition: parts[2].trim(),
            low: parts[3].trim(),
            high: parts[4].trim(),
          });
        }
      }
    });

    if (tempParsed.temp) {
      parsedData = {
        ...tempParsed as WeatherData,
        forecast: forecast
      };
    }

    // Clean the text to remove RAW_DATA and FORECAST lines for display
    const cleanText = text
      .replace(/RAW_DATA\|.*(\n|$)/, '')
      .replace(/FORECAST\|.*(\n|$)/g, '')
      .trim();

    return {
      rawText: cleanText,
      parsedData,
      groundingChunks: groundingChunks as any[], 
    };

  } catch (error) {
    console.error("Gemini API Error:", error);
    throw error;
  }
};