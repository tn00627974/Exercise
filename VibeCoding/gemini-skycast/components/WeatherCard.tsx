import React from 'react';
import { WeatherData } from '../types';
import { Cloud, CloudRain, Sun, Wind, Droplets, MapPin, CloudLightning, CloudSnow, Calendar, Share2 } from 'lucide-react';

interface WeatherCardProps {
  data: WeatherData | null;
  loading: boolean;
  onShare?: () => void;
}

const getWeatherIcon = (condition: string, className: string = "w-16 h-16") => {
  const c = condition.toLowerCase();
  let Icon = Sun;
  let colorClass = "text-yellow-400";

  if (c.includes('雨') || c.includes('rain') || c.includes('drizzle')) {
    Icon = CloudRain;
    colorClass = "text-blue-200";
  } else if (c.includes('雷') || c.includes('storm')) {
    Icon = CloudLightning;
    colorClass = "text-yellow-300";
  } else if (c.includes('雪') || c.includes('snow')) {
    Icon = CloudSnow;
    colorClass = "text-white";
  } else if (c.includes('雲') || c.includes('cloud') || c.includes('陰')) {
    Icon = Cloud;
    colorClass = "text-gray-200";
  }

  return <Icon className={`${className} ${colorClass}`} />;
};

const getBackgroundGradient = (condition: string) => {
    const c = condition.toLowerCase();
    if (c.includes('雨') || c.includes('rain')) return 'from-blue-700 to-blue-900';
    if (c.includes('雲') || c.includes('cloud') || c.includes('陰')) return 'from-gray-500 to-slate-700';
    if (c.includes('雷') || c.includes('storm')) return 'from-indigo-800 to-purple-900';
    if (c.includes('晚') || c.includes('night')) return 'from-indigo-900 to-slate-900';
    return 'from-blue-400 to-blue-600'; // Default Sunny
};

export const WeatherCard: React.FC<WeatherCardProps> = ({ data, loading, onShare }) => {
  if (loading) {
    return (
      <div className="w-full h-96 bg-white/50 backdrop-blur-md rounded-3xl animate-pulse flex flex-col items-center justify-center shadow-xl gap-4">
        <div className="w-16 h-16 bg-white/50 rounded-full"></div>
        <div className="text-gray-500 font-medium">正在為您搜尋最新的天氣資訊...</div>
      </div>
    );
  }

  if (!data) {
    return (
      <div className="w-full h-64 bg-gradient-to-br from-blue-100 to-white rounded-3xl flex items-center justify-center shadow-lg border border-white/50">
        <div className="text-gray-500 flex flex-col items-center gap-2">
          <Sun className="w-8 h-8 text-orange-400" />
          <span>請輸入城市以獲取天氣預報</span>
        </div>
      </div>
    );
  }

  const bgGradient = getBackgroundGradient(data.condition);

  return (
    <div className={`w-full relative overflow-hidden bg-gradient-to-br ${bgGradient} text-white rounded-3xl shadow-2xl p-6 transition-all duration-500`}>
      {/* Decorative circles */}
      <div className="absolute -top-10 -right-10 w-40 h-40 bg-white/10 rounded-full blur-2xl"></div>
      <div className="absolute bottom-10 -left-10 w-32 h-32 bg-white/10 rounded-full blur-xl"></div>

      <div className="relative z-10 flex flex-col h-full justify-between">
        
        {/* Top Row: Location & Date */}
        <div className="flex justify-between items-start">
          <div className="flex items-center gap-2 bg-black/10 backdrop-blur-sm px-3 py-1 rounded-full">
            <MapPin className="w-4 h-4 text-white/80" />
            <span className="font-medium">{data.location}</span>
          </div>
          <div className="flex gap-2">
            {onShare && (
              <button 
                onClick={onShare}
                className="bg-white/20 hover:bg-white/30 backdrop-blur-sm p-1.5 rounded-full transition-colors"
                title="分享至 LINE"
              >
                <Share2 className="w-4 h-4 text-white" />
              </button>
            )}
            <div className="text-white/80 text-sm font-medium py-1">
               即時概況
            </div>
          </div>
        </div>

        {/* Main Temp & Icon */}
        <div className="flex items-center justify-between my-8">
          <div className="flex flex-col">
            <span className="text-7xl font-bold tracking-tighter">
              {data.temp}°
            </span>
            <span className="text-xl font-medium text-white/90 mt-1 pl-1">
              {data.condition}
            </span>
          </div>
          <div className="animate-float">
             {getWeatherIcon(data.condition, "w-20 h-20")}
          </div>
        </div>

        {/* Details Grid */}
        <div className="grid grid-cols-2 gap-4 mb-6">
          <div className="bg-white/10 backdrop-blur-md rounded-xl p-3 flex items-center gap-3 border border-white/10">
            <Droplets className="w-5 h-5 text-blue-200" />
            <div className="flex flex-col">
              <span className="text-xs text-blue-100">濕度</span>
              <span className="font-semibold">{data.humidity}%</span>
            </div>
          </div>
          <div className="bg-white/10 backdrop-blur-md rounded-xl p-3 flex items-center gap-3 border border-white/10">
            <Wind className="w-5 h-5 text-blue-200" />
            <div className="flex flex-col">
              <span className="text-xs text-blue-100">風速</span>
              <span className="font-semibold">{data.wind}</span>
            </div>
          </div>
        </div>

        {/* 7-Day Forecast Section */}
        {data.forecast && data.forecast.length > 0 && (
          <div className="border-t border-white/20 pt-4 mt-2">
            <div className="flex items-center gap-2 mb-3 opacity-80">
                <Calendar className="w-4 h-4" />
                <h4 className="text-xs font-bold uppercase tracking-wider">未來 7 天預報</h4>
            </div>
            
            <div className="flex flex-col gap-2">
              {data.forecast.map((day, idx) => (
                <div key={idx} className="flex items-center justify-between bg-white/5 hover:bg-white/10 transition-colors rounded-lg p-2 px-3">
                  <span className="w-14 font-medium text-sm">{day.day}</span>
                  <div className="flex items-center gap-2 flex-1 justify-center">
                    {getWeatherIcon(day.condition, "w-5 h-5")}
                    <span className="text-xs opacity-90 truncate max-w-[80px]">{day.condition}</span>
                  </div>
                  <div className="flex gap-3 text-sm font-medium w-20 justify-end">
                    <span className="opacity-60">{day.low}°</span>
                    <span>{day.high}°</span>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}

      </div>
    </div>
  );
};