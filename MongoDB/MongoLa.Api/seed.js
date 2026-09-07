// 產生 5 萬筆假發電資料
// 執行方式（PowerShell）:
//   docker cp seed.js mongo:/seed.js
//   docker exec mongo mongosh powerplant --file /seed.js

// 想重跑一次乾淨的，把下面這行的註解拿掉（會清空整個 collection）
// db.readings.drop();

const devices = ["INV-001", "INV-002", "INV-003", "INV-004", "INV-005"];
const docs = [];
const start = Date.UTC(2026, 7, 1, 0, 0, 0);   // 2026-08-01 00:00:00 UTC
const FIVE_MIN_MS = 5 * 60 * 1000;

for (let i = 0; i < 50000; i++) {
  docs.push({
    device_id: devices[i % devices.length],
    timestamp: new Date(start + Math.floor(i / devices.length) * FIVE_MIN_MS),
    power_kw: Math.random() * 20
  });
}

db.readings.insertMany(docs);

print("total: " + db.readings.countDocuments());
print("INV-001: " + db.readings.countDocuments({ device_id: "INV-001" }));
printjson(db.readings.find().sort({ timestamp: 1 }).limit(1).toArray());
printjson(db.readings.find().sort({ timestamp: -1 }).limit(1).toArray());
