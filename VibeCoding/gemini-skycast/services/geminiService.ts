import { GoogleGenAI } from "@google/genai";
import { SearchResult, WeatherData } from "../types";

const ai = new GoogleGenAI({ apiKey: process.env.API_KEY });

export const getWeatherData = async (location: string): Promise<SearchResult> => {
  const modelId = "gemini-2.5-flash"; // Supports Google Search Grounding

  // We ask for a specific format in the first line to parse for the UI widget,
  // followed by natural language for the detailed view.
  const prompt = `
    Find the current weather and 3-day forecast for ${location}.
    
    CRITICAL INSTRUCTION:
    You MUST start your response with a single line strictly following this format:
    RAW_DATA|{Temperature in Celsius (number only)}|{Condition (short text)}|{Humidity (number only)}|{Wind Speed (e.g. 5 km/h)}|{City Name}

    After that line, provide a friendly, helpful weather report in Traditional Chinese (Taiwan usage).
    Include:
    1. A summary of how it feels.
    2. Clothing advice.
    3. A clear list for the next 3 days.
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
    
    // Attempt to parse the first line
    const lines = text.trim().split('\n');
    let parsedData: WeatherData | null = null;
    
    // Look for the RAW_DATA line in the first few lines (in case of whitespace)
    const rawLine = lines.find(line => line.startsWith('RAW_DATA|'));

    if (rawLine) {
      const parts = rawLine.split('|');
      if (parts.length >= 6) {
        parsedData = {
          temp: parts[1].trim(),
          condition: parts[2].trim(),
          humidity: parts[3].trim(),
          wind: parts[4].trim(),
          location: parts[5].trim(),
        };
      }
    }

    // Clean the text to remove the RAW_DATA line for display
    const cleanText = text.replace(/RAW_DATA\|.*(\n|$)/, '').trim();

    return {
      rawText: cleanText,
      parsedData,
      groundingChunks: groundingChunks as any[], // Casting to match our simplified interface if needed
    };

  } catch (error) {
    console.error("Gemini API Error:", error);
    throw error;
  }
};
