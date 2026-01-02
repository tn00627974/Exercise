@echo off
chcp 65001 >nul
setlocal enabledelayedexpansion

echo.
echo ╔════════════════════════════════════════════════════════════╗
echo ║                  Gemini SkyCast 部署工具                     ║
echo ╚════════════════════════════════════════════════════════════╝
echo.

REM 檢查 Node.js
node --version >nul 2>&1
if errorlevel 1 (
    echo ❌ 錯誤：未安裝 Node.js
    echo 請從 https://nodejs.org 下載並安裝
    pause
    exit /b 1
)

echo ✅ Node.js 已安裝
echo.

REM 選單
echo 請選擇操作：
echo.
echo 1) 只構建生產版本 (npm run build)
echo 2) 構建並內網部署 (構建 + 啟動服務器)
echo 3) 只啟動現有服務器
echo 4) 檢查 API Key 配置
echo.

set /p choice="請輸入選項 (1-4): "

if "%choice%"=="1" (
    echo.
    echo 🔨 開始構建...
    call npm run build
    if errorlevel 1 (
        echo ❌ 構建失敗
        pause
        exit /b 1
    )
    echo.
    echo ✅ 構建完成！dist 文件夾已準備好
    echo 📁 位置: %cd%\dist
    pause
    exit /b 0
)

if "%choice%"=="2" (
    echo.
    echo 🔨 開始構建...
    call npm run build
    if errorlevel 1 (
        echo ❌ 構建失敗
        pause
        exit /b 1
    )
    echo ✅ 構建完成
    echo.
    echo 🚀 正在啟動服務器...
    
    REM 檢查 http-server 是否已安裝
    npx http-server --version >nul 2>&1
    if errorlevel 1 (
        echo 📦 正在安裝 http-server...
        call npm install -g http-server
    )
    
    REM 獲取 IP 地址
    setlocal enabledelayedexpansion
    for /f "tokens=2 delims=: " %%a in ('ipconfig ^| find /i "ipv4"') do (
        set "IP=%%a"
    )
    
    echo.
    echo ╔════════════════════════════════════════════════════════════╗
    echo ║                  ✅ 服務器已啟動！                           ║
    echo ╚════════════════════════════════════════════════════════════╝
    echo.
    echo 📱 本地訪問:   http://localhost:8080
    echo 🌐 內網分享:   http://!IP:~1!:8080
    echo.
    echo 💡 分享上面的內網地址給其他人即可訪問
    echo.
    echo 按 Ctrl+C 停止服務器
    echo.
    
    cd dist
    npx http-server -p 8080
    exit /b 0
)

if "%choice%"=="3" (
    echo.
    echo 🚀 正在啟動服務器...
    
    if not exist "dist\" (
        echo ❌ 錯誤：dist 文件夾不存在
        echo 請先執行選項 1 或 2 進行構建
        pause
        exit /b 1
    )
    
    REM 獲取 IP 地址
    setlocal enabledelayedexpansion
    for /f "tokens=2 delims=: " %%a in ('ipconfig ^| find /i "ipv4"') do (
        set "IP=%%a"
    )
    
    echo.
    echo ╔════════════════════════════════════════════════════════════╗
    echo ║                  ✅ 服務器已啟動！                           ║
    echo ╚════════════════════════════════════════════════════════════╝
    echo.
    echo 📱 本地訪問:   http://localhost:8080
    echo 🌐 內網分享:   http://!IP:~1!:8080
    echo.
    echo 按 Ctrl+C 停止服務器
    echo.
    
    cd dist
    npx http-server -p 8080
    exit /b 0
)

if "%choice%"=="4" (
    echo.
    echo 🔐 檢查 API Key 配置...
    echo.
    
    if not exist ".env.local" (
        echo ❌ .env.local 文件不存在
    ) else (
        for /f "delims== tokens=1,2" %%A in (.env.local) do (
            if "%%A"=="GEMINI_API_KEY" (
                if "%%B"=="" (
                    echo ⚠️  GEMINI_API_KEY 未設置（空值）
                ) else if "%%B"=="PLACEHOLDER_API_KEY" (
                    echo ❌ GEMINI_API_KEY 仍為佔位符
                    echo.
                    echo 📝 請按以下步驟配置：
                    echo 1. 訪問 https://aistudio.google.com/apikey
                    echo 2. 點擊 "Create API key"
                    echo 3. 複製密鑰字符串
                    echo 4. 編輯 .env.local，替換為您的密鑰
                ) else (
                    echo ✅ GEMINI_API_KEY 已配置
                    echo 值: %%B
                )
            )
        )
    )
    echo.
    pause
    exit /b 0
)

echo ❌ 無效的選項
pause
exit /b 1
