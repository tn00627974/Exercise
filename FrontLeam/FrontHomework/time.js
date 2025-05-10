// 這段程式碼會在網頁載入後自動更新當前時間
function updateTime() {
    const now = new Date();
    const year = now.getFullYear();
    const month = String(now.getMonth() + 1).padStart(2, '0');
    const day = String(now.getDate()).padStart(2, '0');
    const hour = String(now.getHours()).padStart(2, '0');
    const minute = String(now.getMinutes()).padStart(2, '0');
    const second = String(now.getSeconds()).padStart(2, '0');
    
    const formatted = `${year}/${month}/${day} ${hour}:${minute}:${second}`;
    document.getElementById('nowTime').innerText = formatted;
}

// 先呼叫一次
updateTime();

// 每秒更新一次
setInterval(updateTime, 1000);