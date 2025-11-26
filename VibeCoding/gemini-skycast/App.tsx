import React, { useState, useEffect } from 'react';
import { Search, ExternalLink, Sparkles, AlertCircle } from 'lucide-react';
import { getWeatherData } from './services/geminiService';
import { WeatherData, GroundingChunk } from './types';
import { WeatherCard } from './components/WeatherCard';

const App = () => {
  const [city, setCity] = useState('台北');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [weatherData, setWeatherData] = useState<WeatherData | null>(null);
  const [rawResponse, setRawResponse] = useState<string>('');
  const [groundingSources, setGroundingSources] = useState<GroundingChunk[]>([]);

  const handleSearch = async (e?: React.FormEvent) => {
    if (e) e.preventDefault();
    if (!city.trim()) return;

    setLoading(true);
    setError(null);
    setWeatherData(null);
    setRawResponse('');
    setGroundingSources([]);

    try {
      const result = await getWeatherData(city);
      setWeatherData(result.parsedData);
      setRawResponse(result.rawText);
      setGroundingSources(result.groundingChunks);
    } catch (err) {
      setError("無法獲取天氣資訊，請稍後再試。");
    } finally {
      setLoading(false);
    }
  };

  // Initial load
  useEffect(() => {
    handleSearch();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  return (
    <div className="min-h-screen bg-slate-50 flex items-center justify-center p-4 md:p-6">
      <div className="w-full max-w-md flex flex-col gap-6">
        
        {/* Header */}
        <div className="flex flex-col gap-1">
          <div className="flex items-center gap-2 text-blue-600">
            <Sparkles className="w-5 h-5" />
            <h1 className="font-bold tracking-tight text-sm uppercase">Gemini SkyCast</h1>
          </div>
          <h2 className="text-2xl font-bold text-slate-900">智能天氣預報</h2>
        </div>

        {/* Search Input */}
        <form onSubmit={handleSearch} className="relative group">
          <input
            type="text"
            value={city}
            onChange={(e) => setCity(e.target.value)}
            placeholder="輸入城市 (例如: 東京, 紐約)"
            className="w-full h-12 pl-12 pr-4 bg-white rounded-2xl border-2 border-slate-100 text-slate-900 placeholder-slate-400 focus:outline-none focus:border-blue-500 focus:ring-4 focus:ring-blue-500/10 transition-all shadow-sm group-hover:shadow-md"
            disabled={loading}
          />
          <Search className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-400 group-focus-within:text-blue-500 transition-colors" />
          <button 
            type="submit" 
            disabled={loading || !city}
            className="absolute right-2 top-1/2 -translate-y-1/2 bg-blue-600 hover:bg-blue-700 text-white p-1.5 rounded-xl disabled:opacity-50 transition-colors"
          >
            <Search className="w-4 h-4" />
          </button>
        </form>

        {/* Error Message */}
        {error && (
          <div className="bg-red-50 text-red-600 p-4 rounded-2xl flex items-center gap-3 text-sm border border-red-100">
            <AlertCircle className="w-5 h-5 flex-shrink-0" />
            {error}
          </div>
        )}

        {/* Main Weather Card */}
        <WeatherCard data={weatherData} loading={loading} />

        {/* Analysis / Forecast Text */}
        {(rawResponse || loading) && (
          <div className="bg-white rounded-3xl p-6 shadow-sm border border-slate-100">
            <h3 className="text-sm font-bold text-slate-400 uppercase tracking-wider mb-4 flex items-center gap-2">
              <Sparkles className="w-4 h-4" /> 
              Gemini 分析
            </h3>
            
            {loading ? (
              <div className="space-y-3">
                <div className="h-4 bg-slate-100 rounded w-3/4 animate-pulse"></div>
                <div className="h-4 bg-slate-100 rounded w-full animate-pulse"></div>
                <div className="h-4 bg-slate-100 rounded w-5/6 animate-pulse"></div>
              </div>
            ) : (
              <div className="prose prose-slate prose-sm max-w-none text-slate-600 leading-relaxed whitespace-pre-wrap">
                {rawResponse}
              </div>
            )}
          </div>
        )}

        {/* Sources (Grounding) */}
        {!loading && groundingSources.length > 0 && (
          <div className="flex flex-col gap-3">
            <h4 className="text-xs font-semibold text-slate-400 uppercase tracking-wider pl-1">
              資料來源 (Google Search)
            </h4>
            <div className="flex flex-wrap gap-2">
              {groundingSources.map((chunk, idx) => {
                if (!chunk.web) return null;
                return (
                  <a
                    key={idx}
                    href={chunk.web.uri}
                    target="_blank"
                    rel="noopener noreferrer"
                    className="flex items-center gap-1.5 bg-white px-3 py-1.5 rounded-lg border border-slate-200 text-xs font-medium text-slate-600 hover:text-blue-600 hover:border-blue-200 hover:shadow-sm transition-all truncate max-w-full"
                  >
                    <ExternalLink className="w-3 h-3 flex-shrink-0" />
                    <span className="truncate max-w-[200px]">{chunk.web.title}</span>
                  </a>
                );
              })}
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default App;
