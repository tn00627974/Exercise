import React, { useState } from 'react';
import { Download, Share2, Zap, Check } from 'lucide-react';

export const DeploymentHelper: React.FC = () => {
  const [status, setStatus] = useState<'idle' | 'building' | 'success' | 'error'>('idle');
  const [message, setMessage] = useState('');
  const [ipAddress, setIpAddress] = useState('');

  // 獲取本地 IP
  const getLocalIP = async () => {
    try {
      const response = await fetch('/api/ip');
      const data = await response.json();
      setIpAddress(data.ip);
    } catch (error) {
      setIpAddress('192.168.x.x');
    }
  };

  // 構建項目
  const handleBuild = async () => {
    setStatus('building');
    setMessage('正在構建項目...');
    
    try {
      const response = await fetch('/api/build', { method: 'POST' });
      if (response.ok) {
        setStatus('success');
        setMessage('✅ 構建完成！dist 文件夾已準備好部署');
        getLocalIP();
      } else {
        setStatus('error');
        setMessage('❌ 構建失敗，請檢查控制台');
      }
    } catch (error) {
      setStatus('error');
      setMessage('❌ 請求失敗');
    }
  };

  // 啟動服務器
  const handleDeploy = async () => {
    setStatus('building');
    setMessage('正在啟動內網服務器...');
    
    try {
      const response = await fetch('/api/deploy', { method: 'POST' });
      if (response.ok) {
        setStatus('success');
        getLocalIP();
        setMessage(`✅ 服務器已啟動！內網地址: http://${ipAddress}:8080`);
      } else {
        setStatus('error');
        setMessage('❌ 啟動失敗');
      }
    } catch (error) {
      setStatus('error');
      setMessage('❌ 請求失敗');
    }
  };

  return (
    <div className="fixed bottom-4 right-4 w-96 bg-white rounded-lg shadow-2xl p-6 border-2 border-blue-500">
      <h2 className="text-xl font-bold mb-4 flex items-center gap-2">
        <Zap className="text-yellow-500" size={24} />
        部署助手
      </h2>

      <div className="space-y-3">
        {/* 構建按鈕 */}
        <button
          onClick={handleBuild}
          disabled={status === 'building'}
          className="w-full bg-blue-500 hover:bg-blue-600 disabled:bg-gray-400 text-white font-bold py-3 px-4 rounded-lg flex items-center justify-center gap-2 transition"
        >
          <Download size={20} />
          {status === 'building' && status.includes('構建') ? '正在構建...' : '1. 構建生產版本'}
        </button>

        {/* 部署按鈕 */}
        <button
          onClick={handleDeploy}
          disabled={status === 'building'}
          className="w-full bg-green-500 hover:bg-green-600 disabled:bg-gray-400 text-white font-bold py-3 px-4 rounded-lg flex items-center justify-center gap-2 transition"
        >
          <Share2 size={20} />
          {status === 'building' && status.includes('啟動') ? '正在啟動...' : '2. 內網部署'}
        </button>

        {/* 狀態消息 */}
        {message && (
          <div className={`p-3 rounded-lg font-semibold ${
            status === 'success' ? 'bg-green-100 text-green-800' :
            status === 'error' ? 'bg-red-100 text-red-800' :
            'bg-blue-100 text-blue-800'
          }`}>
            {message}
          </div>
        )}

        {/* 分享信息 */}
        {status === 'success' && ipAddress && (
          <div className="bg-purple-100 p-4 rounded-lg border-2 border-purple-300">
            <p className="font-bold text-purple-800 mb-2">🔗 內網分享地址：</p>
            <p className="text-lg font-mono text-purple-900 break-all">
              http://{ipAddress}:8080
            </p>
            <button
              onClick={() => {
                navigator.clipboard.writeText(`http://${ipAddress}:8080`);
                alert('✅ 已複製到剪貼板');
              }}
              className="mt-2 w-full bg-purple-500 hover:bg-purple-600 text-white py-2 px-3 rounded flex items-center justify-center gap-2"
            >
              <Check size={16} />
              複製分享地址
            </button>
          </div>
        )}
      </div>

      <p className="text-xs text-gray-500 mt-4 text-center">
        💡 提示：確保 API Key 已在 .env.local 中配置
      </p>
    </div>
  );
};
