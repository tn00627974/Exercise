#!/usr/bin/env node

const http = require('http');
const { exec } = require('child_process');
const path = require('path');
const os = require('os');
const fs = require('fs');

// 獲取本地 IP
function getLocalIP() {
  const interfaces = os.networkInterfaces();
  for (const name of Object.keys(interfaces)) {
    for (const iface of interfaces[name]) {
      if (iface.family === 'IPv4' && !iface.internal) {
        return iface.address;
      }
    }
  }
  return '127.0.0.1';
}

// 執行命令
function runCommand(command) {
  return new Promise((resolve, reject) => {
    exec(command, { cwd: __dirname }, (error, stdout, stderr) => {
      if (error) {
        reject(error);
      } else {
        resolve(stdout);
      }
    });
  });
}

// 創建 HTTP 服務器
const server = http.createServer(async (req, res) => {
  res.setHeader('Access-Control-Allow-Origin', '*');
  res.setHeader('Content-Type', 'application/json');

  if (req.url === '/api/ip' && req.method === 'GET') {
    res.writeHead(200);
    res.end(JSON.stringify({ ip: getLocalIP() }));
  } 
  else if (req.url === '/api/build' && req.method === 'POST') {
    try {
      console.log('開始構建...');
      await runCommand('npm run build');
      res.writeHead(200);
      res.end(JSON.stringify({ success: true, message: '構建完成' }));
    } catch (error) {
      res.writeHead(500);
      res.end(JSON.stringify({ success: false, error: error.message }));
    }
  } 
  else if (req.url === '/api/deploy' && req.method === 'POST') {
    try {
      console.log('啟動服務器...');
      const ip = getLocalIP();
      // 在後台啟動 http-server
      exec('npx http-server dist -p 8080', { cwd: __dirname });
      res.writeHead(200);
      res.end(JSON.stringify({ 
        success: true, 
        message: '服務器已啟動',
        url: `http://${ip}:8080`
      }));
    } catch (error) {
      res.writeHead(500);
      res.end(JSON.stringify({ success: false, error: error.message }));
    }
  }
  else {
    res.writeHead(404);
    res.end(JSON.stringify({ error: '未找到' }));
  }
});

const PORT = 3456;
server.listen(PORT, () => {
  console.log(`✅ 部署服務器運行在 http://localhost:${PORT}`);
});
