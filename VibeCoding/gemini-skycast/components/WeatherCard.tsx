import React from 'react';
import { WeatherData } from '../types';
import { Cloud, CloudRain, Sun, Wind, Droplets, MapPin, CloudLightning, CloudSnow } from 'lucide-react';

interface WeatherCardProps {
  data: WeatherData | null;
  loading: boolean;
}

const getWeatherIcon = (condition: string) => {
  const c = condition.toLowerCase();
  if (c.includes('雨') || c.includes('rain') || c.includes('drizzle')) return <CloudRain className="w-16 h-16 text-blue-200" />;
  if (c.includes('雷') || c.includes('storm')) return <CloudLightning className="w-16 h-16 text-yellow-300" />;
  if (c.includes('雪') || c.includes('snow')) return <CloudSnow className="w-16 h-16 text-white" />;
  if (c.includes('雲') || c.includes('cloud') || c.includes('陰')) return <Cloud className="w-16 h-16 text-gray-200" />;
  return <Sun className="w-16 h-16 text-yellow-400" />;
};

const getBackgroundGradient = (condition: string) => {
    const c = condition.toLowerCase();
    if (c.includes('雨') || c.includes('rain')) return 'from-blue-700 to-blue-900';
    if (c.includes('雲') || c.includes('cloud') || c.includes('陰')) return 'from-gray-500 to-slate-700';
    if (c.includes('雷') || c.includes('storm')) return 'from-indigo-800 to-purple-900';
    if (c.includes('晚') || c.includes('night')) return 'from-indigo-900 to-slate-900';
    return 'from-blue-400 to-blue-600'; // Default Sunny
};

export const WeatherCard: React.FC<WeatherCardProps> = ({ data, loading }) => {
  if (loading) {
    return (
      <div className="w-full h-64 bg-white/50 backdrop-blur-md rounded-3xl animate-pulse flex items-center justify-center shadow-xl">
        <div className="text-gray-500 font-medium">正如火如荼地搜索最新的天氣資訊...</div>
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
          <div className="text-white/80 text-sm font-medium">
             即時概況
          </div>
        </div>

        {/* Middle Row: Big Temp & Icon */}
        <div className="flex items-center justify-between my-6">
          <div className="flex flex-col">
            <span className="text-7xl font-bold tracking-tighter">
              {data.temp}°
            </span>
            <span className="text-xl font-medium text-white/90 mt-1 pl-1">
              {data.condition}
            </span>
          </div>
          <div className="animate-float">
             {getWeatherIcon(data.condition)}
          </div>
        </div>

        {/* Bottom Row: Details */}
        <div className="grid grid-cols-2 gap-4">
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
      </div>
    </div>
  );
};
