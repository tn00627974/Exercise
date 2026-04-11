# Oracle Cloud Always Free 佈署教學

這份文件說明如何把目前的台股 RSS Discord Bot 佈署到 Oracle Cloud Always Free VM，並用 systemd 讓 Bot 開機自動啟動、異常自動重啟。

適用情境：

- 想要 24/7 常駐執行，不依賴本機電腦
- 想使用 Oracle Cloud 免費額度
- 專案主程式為 `python bot.py`

---

## 1. 先備條件

在開始之前，請先準備好以下資料：

- Oracle Cloud 帳號
- 已建立好的 Discord Bot Token
- Discord 頻道 ID
- RSS URL
- GitHub 專案位址，或你要上傳到主機的程式碼

本專案必要環境變數如下：

```env
DISCORD_TOKEN=你的 Discord Bot Token
CHANNEL_ID=你的 Discord 頻道 ID
RSS_URL=https://tw.stock.yahoo.com/rss?category=tw-market
MENTION_USER_ID=
```

如果你要一次管理多個頻道或多個 RSS，建議改用：

```env
SUBSCRIPTIONS_FILE=subscriptions.json
```

---

## 2. 在 Oracle Cloud 建立免費 VM

### 2.1 建立 Instance

1. 登入 Oracle Cloud Console。
2. 進入 `Compute` -> `Instances`。
3. 點選 `Create instance`。
4. 名稱可填：`dc-rss-bot`。
5. 映像檔建議選：`Ubuntu 24.04` 或 `Ubuntu 22.04`。
6. Shape 建議選 **Always Free** 可用的：
   - 首選：`Instance type > Virtual machine > Ampere > VM.Standard.A1.Flex`
   - 建議配置：`1 OCPU`、`6 GB RAM` 以上
   - Boot volume 建議至少 `50 GB`，Always Free 額度內通常足夠
   - 若區域沒資源，可退而求其次選：`Instance type > Virtual machine > Specialty and previous generation > VM.Standard.E2.1.Micro`

### 2.1.1 Primary network 怎麼選

建立 VM 時，如果你看到以下選項：

- `Select existing virtual cloud network`
- `Create new virtual cloud network`
- `Specify OCID`

對這個 Discord Bot，建議直接選：

- `Create new virtual cloud network`

VNIC 名稱可填：`dc-rss-bot-vcn`


原因是這個選項最省事，Oracle 會一起幫你建立好基本網路設定，包含：

- VCN
- Subnet
- Route table
- Internet gateway

這樣後面要用 SSH 連進 VM，通常最不容易卡住。

建議搭配以下設定：

- Subnet：選 public subnet
- Public IPv4 address：選自動指派
- Security rules：至少允許 SSH 的 `22` port

其他兩個選項適用情境如下：

- `Select existing virtual cloud network`：你已經有自己建立好的 VCN，想讓這台 VM 併入既有網路
- `Specify OCID`：你是在做自動化部署、Terraform、或需要精準指定既有網路資源時才使用

如果你只是第一次部署這個 Bot，不需要額外建立 Secondary VNIC，也不需要做多網卡配置，一張 Primary VNIC 就夠了。

### 2.2 設定 SSH Key

建立主機時，於 SSH keys 選擇以下其一：

- 上傳你現有的公鑰
- 由 Oracle 幫你產生金鑰後下載私鑰

建議使用你自己的公私鑰，後續管理比較方便。

Windows 若尚未建立 SSH 金鑰，可在 PowerShell 執行：

```powershell
ssh-keygen -t ed25519 -C "oracle-bot"
```

公鑰通常位於：

```powershell
$env:USERPROFILE\.ssh\id_ed25519.pub
```

看到以下選項：

- `Generate a key pair for me`
- `Upload public key file (.pub)`
- `Paste public key`
- `No SSH keys`

- 建議直接選(擇一)：

- `Upload public key file (.pub)` (檔案路徑:$env:USERPROFILE\.ssh\id_ed25519.pub)
- `Paste public key` (打開檔案後直接複製貼上)

## 3. 連線到 Oracle VM

建立完成後，記下該 VM 的 Public IP。

在 Windows PowerShell 執行：

```powershell
ssh ubuntu@你的_VM_Public_IP
```

如果你使用 Oracle 下載的私鑰，連線方式類似：

```powershell
ssh -i C:\path\to\private.key ubuntu@你的_VM_Public_IP
```

第一次連線看到指紋確認訊息時，輸入：

```text
yes
```

---

## 4. 更新系統並安裝必要套件

登入 VM 後，先執行：


```bash
sudo apt update && sudo apt upgrade -y # 更新套件 
sudo apt install -y python3 python3-venv python3-pip git curl # 安裝python 和 git
```

確認 Python 版本：

```bash
python3 --version
```

建議至少看到 Python 3.10 以上。

---

## 5. 取得專案程式碼

### 方式 A：直接從 GitHub 下載

如果你的專案已放到 GitHub：

```bash
cd ~
git clone https://github.com/tn00627974/Exercise.git
cd Exercise/Discord/DcBot
```

如果你之後改了自己的 repo，請把網址換成你的實際 repository。

### 方式 B：從本機上傳

如果你不想用 GitHub，也可以用 SCP 或 VS Code Remote SSH 上傳整個專案資料夾到主機。

Windows PowerShell 可直接使用 `scp`，例如：

```powershell
scp -i $env:USERPROFILE\.ssh\id_ed25519 -r "D:\工程師資料夾\Exercise\Discord\DcBot" ubuntu@你的_VM_Public_IP:~
```

如果你的 SSH 私鑰就是預設路徑，有時也可以省略 `-i`：

```powershell
scp -r "D:\工程師資料夾\Exercise\Discord\DcBot" ubuntu@你的_VM_Public_IP:~
```

上傳完成後，再 SSH 進 VM：

```powershell
ssh -i $env:USERPROFILE\.ssh\id_ed25519 ubuntu@你的_VM_Public_IP
```

然後在 VM 內確認檔案：

```bash
cd ~/DcBot
ls -a
```

注意：不要直接沿用從 Windows 上傳的 `.venv`。
Windows 的虛擬環境通常會是 `.venv/Scripts` 和 `python.exe`，Ubuntu 不能直接使用，請在 Linux 主機上刪除後重建。

上傳後請確定主程式位於：

```bash
~/DcBot/bot.py
```

或其他你實際使用的路徑。

---

## 6. 建立虛擬環境並安裝依賴

在專案目錄執行：

```bash
rm -rf .venv # 若用windows scp方式，需刪除資料夾下的venv虛擬，因為Ubuntu不能使用，需重安裝!!
python3 -m venv .venv
source .venv/bin/activate
pip install --upgrade pip
pip install -r requirements.txt
```

如果你是用 `scp` 從 Windows 把整個專案資料夾傳上來，先執行 `rm -rf .venv` 很重要，因為 Windows 建立的虛擬環境不能直接在 Ubuntu 使用。

安裝完成後可先確認：

```bash
pip list
```

應至少包含：

- discord.py
- feedparser
- python-dotenv
- aiohttp

---

## 7. 設定環境變數

你有兩種做法，推薦第 1 種。

### 做法 1：使用 `.env` 檔案

在專案目錄建立 `.env`：

```bash
nano .env
```

填入內容：

```env
DISCORD_TOKEN=你的 Discord Bot Token
CHANNEL_ID=你的 Discord 頻道 ID
RSS_URL=https://tw.stock.yahoo.com/rss?category=tw-market
MENTION_USER_ID=
PORT=10000
```

如果你要多組訂閱，改成：

```env
DISCORD_TOKEN=你的 Discord Bot Token
SUBSCRIPTIONS_FILE=subscriptions.json
PORT=10000
```

這個專案已內建 `load_dotenv()`，因此直接放 `.env` 就能被讀取。

### 做法 2：systemd 使用獨立環境檔

如果你不想把敏感資訊放在專案資料夾，可改放到：

```bash
sudo nano /etc/dc-rss-bot.env
```

內容範例：

```env
DISCORD_TOKEN=你的 Discord Bot Token
SUBSCRIPTIONS_FILE=subscriptions.json
PORT=10000
```

如果你使用這種方式，稍後的 systemd service 要用 `EnvironmentFile=` 指向這個檔案。

---

## 8. 先手動測試 Bot 是否可正常啟動

在專案目錄執行：

```bash
source .venv/bin/activate
python bot.py --test "Oracle Cloud 部署測試成功"
```

如果 Discord 頻道有收到訊息，代表：

- Token 正確
- Bot 已加入伺服器
- 頻道 ID 正確
- Bot 具有發送訊息權限

若你要測試正式模式是否能啟動，可再執行：

```bash
python bot.py
```

看到類似以下訊息表示正常：

```text
[INFO] Health check server running on port 10000
[INFO] Primed 50 items from RSS
[INFO] Shard ID None has connected to Gateway
```

確認正常後，按 `Ctrl + C` 停掉，接著再做 systemd 常駐。

---

## 9. 設定 systemd 讓 Bot 常駐執行

### 9.1 建立 service 檔

```bash
sudo nano /etc/systemd/system/dc-rss-bot.service
```

填入以下內容：

```ini
[Unit]
Description=Discord RSS Bot
After=network-online.target
Wants=network-online.target

[Service]
Type=simple
User=ubuntu
WorkingDirectory=/home/ubuntu/DcBot
ExecStart=/home/ubuntu/DcBot/.venv/bin/python bot.py
Restart=always
RestartSec=10

# 如果你使用 /etc/dc-rss-bot.env，取消下一行註解並刪除開頭的 #
# EnvironmentFile=/etc/dc-rss-bot.env

[Install]
WantedBy=multi-user.target
```

如果你的專案不是放在 `/home/ubuntu/DcBot`，請改成你的實際路徑。

### 9.2 重新載入並啟用服務

```bash
sudo systemctl daemon-reload # 
sudo systemctl enable dc-rss-bot #
sudo systemctl start dc-rss-bot # 
```

### 9.3 檢查服務狀態

```bash
sudo systemctl status dc-rss-bot
```

若看到 `active (running)` 就代表已成功常駐。

---

## 10. 查看執行日誌

即時查看日誌：

```bash
sudo journalctl -u dc-rss-bot -f
```

查看最近 100 行：

```bash
sudo journalctl -u dc-rss-bot -n 100 --no-pager
```

正常情況下，應能看到：

```text
Health check server running on port 10000
Primed 50 items from RSS
Shard ID None has connected to Gateway
```

---

## 11. 開機自動啟動與重啟管理

常用指令如下：

```bash
sudo systemctl restart dc-rss-bot
sudo systemctl stop dc-rss-bot
sudo systemctl start dc-rss-bot
sudo systemctl status dc-rss-bot
```

因為 service 設定了：

```ini
Restart=always
```

所以程式異常退出時，systemd 會自動重啟。

---

## 12. Oracle Cloud 網路與防火牆建議

這個 Bot 的主要用途是主動連出到 Discord 與 RSS，不一定需要對外開放 HTTP 服務。

### 建議做法

- 對外只開 `22` port 給 SSH
- `PORT=10000` 只作為本機健康檢查用途即可
- 不必為了這個 Bot 特別把 `10000` 對外開放

### 在 Ubuntu 啟用 UFW

```bash
sudo apt install -y ufw
sudo ufw allow OpenSSH
sudo ufw enable
sudo ufw status
```

### Oracle Security List / Network Security Group

請確認 Oracle 雲端網路規則至少允許：

- Ingress TCP 22：供 SSH 連線
- Egress 全開：讓主機可連外到 Discord 與 RSS 來源

如果你沒有要對外提供網站，不需要額外放行 80、443、10000。

---

## 13. 之後如何更新程式

如果你是用 GitHub clone：

```bash
cd ~/Exercise/Discord/DcBot
git pull
source .venv/bin/activate
pip install -r requirements.txt
sudo systemctl restart dc-rss-bot
```

更新後再檢查：

```bash
sudo systemctl status dc-rss-bot
sudo journalctl -u dc-rss-bot -n 50 --no-pager
```

---

## 14. 常見問題排查

### 1. `Missing environment variable: DISCORD_TOKEN`

代表 `.env` 沒有被正確讀到，或 systemd 沒帶入環境變數。

檢查方向：

- 專案目錄是否真的有 `.env`
- `WorkingDirectory` 是否正確
- 若用 `EnvironmentFile=`，檔案路徑是否正確

### 2. `Channel not found`

通常是以下原因：

- `CHANNEL_ID` 填錯
- Bot 沒被邀請進伺服器
- Bot 沒有查看頻道權限

### 3. `Forbidden sending to channel`

代表 Bot 在該頻道沒有發文權限。

請檢查 Discord 頻道權限：

- View Channel
- Send Messages
- Embed Links（若未來要送嵌入訊息）

### 4. RSS 能抓到，但沒有推播舊文

這是正常行為。

此專案在啟動時會先把目前 RSS 文章標記為已讀，只推播之後的新文章，避免啟動時洗版。

### 5. 主機重開後 Bot 沒有啟動

請確認：

```bash
sudo systemctl is-enabled dc-rss-bot
```

若不是 `enabled`，執行：

```bash
sudo systemctl enable dc-rss-bot
```

---

## 15. 建議的最終目錄結構

```text
/home/ubuntu/Exercise/Discord/DcBot
|- bot.py
|- requirements.txt
|- subscriptions.json
|- .env
|- .venv/
```

---

## 16. 最短部署流程總結

如果你要快速照做，最短流程如下：

```bash
sudo apt update && sudo apt upgrade -y
sudo apt install -y python3 python3-venv python3-pip git
cd ~
git clone https://github.com/tn00627974/Exercise.git
cd Exercise/Discord/DcBot
python3 -m venv .venv
source .venv/bin/activate
pip install --upgrade pip
pip install -r requirements.txt
nano .env
python bot.py --test "Oracle Cloud 部署測試成功"
sudo nano /etc/systemd/system/dc-rss-bot.service
sudo systemctl daemon-reload
sudo systemctl enable dc-rss-bot
sudo systemctl start dc-rss-bot
sudo journalctl -u dc-rss-bot -f
```

---

## 17. 補充建議

- Oracle Always Free 不會像部分免費平台一樣自動休眠，適合這種長駐 Bot。
- 建議定期執行 `sudo apt update && sudo apt upgrade -y` 做安全更新。
- 若你之後要管理多組 RSS/頻道，優先使用 `subscriptions.json` 搭配 `SUBSCRIPTIONS_FILE`，維護會比單一環境變數更方便。
