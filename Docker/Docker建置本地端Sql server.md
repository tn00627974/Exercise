
![](https://i.imgur.com/JA6CtmR.png)

[Microsoft SQL Server - 基於 Ubuntu 的映像](https://mcr.microsoft.com/en-us/artifact/mar/mssql/server/about)

## Docker 

打開`cmd` or `powershell`

現在您可以在 Ubuntu 22.04 上開始使用 SQL Server 2022。若要部署基於 Ubuntu 22.04 的 SQL Server 2022 的容器，請使用下列命令：

```cmd
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=yourStrong(!)Password" -e "MSSQL_PID=Evaluation" -p 1434:1433 --name sqlserver3 --hostname sqlserver3 -d mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04
```

`docker run` 創建並啟動一個新的 Docker 容器的指令

`-e "ACCEPT_EULA=Y` 
設置環境變數，用來表示你已經接受了 SQL Server 的最終使用者授權合約 (End User License Agreement, EULA)。如果不設置這個變數，容器將無法啟動。

`-e MSSQL_SA_PASSWORD`
sqlserver的密碼 : yourStrong(!)Password (帳號預設sa)
若要**更改密碼**需要滿足以下要求：
- 至少 8 個字符長。
- 包含大小寫字母、數字和符號（即混合三種以上類型的字符）。

`-e MSSQL_PID` 
指定 SQL Server 的產品版本
指定了 `Evaluation` (SQL Server 的評估版本（即免費的試用版本，有時間限制)
可用的選項包括：
- `Evaluation`：評估版，有 180 天的試用期。
- `Developer`：免費的開發者版本，功能完全相同，但不適用於生產環境。
- `Express`：免費的精簡版，有限制功能和性能。
- 也可以使用購買的產品 ID 來指定其他版本，如 Standard 或 Enterprise。

`-p 1434:1433`
- `-p` 用來將 Docker 容器的內部端口映射到主機的端口。格式為 `<host_port>:<container_port>`。
    - `1433` 是 SQL Server 在容器內部的預設連接端口。
    - `1434` 是你在主機上打開的端口，用來與 SQL Server 容器通信。這意味著你在本機或外部連接到 SQL Server 時，需要使用主機的 `1434` 端口。
- 當容器在主機上啟動後，SQL Server 將會在主機的 `1434` 端口上提供服務。

`--name sqlserver3`
- `--name` 用來指定 Docker 容器的名稱，這裡設置為 `sqlserver3`，方便管理和辨識容器。你可以通過這個名稱來運行、停止或查看容器。

`--hostname sqlserver3`
- `--hostname` 是用來設置容器內部的主機名稱，這是容器內部系統識別自己的名稱。這裡設定為 `sqlserver3`，和容器名稱相同，但你也可以選擇不同的名稱。

`-d`
- `-d` 表示以**分離模式**（detached mode）運行容器。這意味著容器會在背景運行，你不會看到容器的輸出，但容器仍在運行。

`mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04`
- 這個部分是指定所要運行的 SQL Server Docker 映像檔（image）。具體內容為：
    - `mcr.microsoft.com` 是微軟容器註冊庫（Microsoft Container Registry）。
    - `mssql/server` 是 SQL Server 的映像檔。
    - `2022-CU14-ubuntu-22.04` 是映像檔的標籤，表示你使用的是 SQL Server 2022 的第 14 個累積更新 (CU14)，並且該映像基於 Ubuntu 22.04 Linux 系統。


- 打開Docker Contauners 可以看到 sqlserver3 運行中..
![](https://i.imgur.com/zWvwNU3.png)


## 接著使用SSMS進行連線

根據你目前的設定，`sqlserver3` 容器已經將 SQL Server 的埠從 1433 綁定到主機的 1434。以下是使用 SQL Server Management Studio (SSMS) 連接 `sqlserver3` 容器的步驟：

### 1. 開啟 SSMS
在你的本地電腦上，開啟 SQL Server Management Studio。

### 2. 連線到 SQL Server
在 SSMS 的「連線到伺服器」對話框中，請按照以下步驟進行設定：
因為我們端口有指定`-p 1434 : 1433` ,所以伺服器要加上1434端口
- **伺服器名稱**：`localhost,1434`
- **驗證**：選擇 **SQL Server 驗證**
- **登入名稱**：`sa`
- **密碼**：你在創建 Docker 容器時設定的 `MSSQL_SA_PASSWORD`（例如 `YourStrongPassword1!`）

### 3. 測試連線
按下「連線」按鈕。如果一切正常，你應該能夠成功連線到運行在 Docker 容器中的 SQL Server 2022。

### 補充說明：
- 如果你在遠端主機上運行 Docker 容器，請將 `localhost` 替換為該主機的 IP 地址，例如：`<主機 IP>,1434`。
- 如果連線過程中遇到問題，請檢查防火牆是否允許 1434 埠的連線。



# Docker Volumes 保存數據內容

https://www.youtube.com/watch?v=-pzptvcJNh0
https://larrylu.blog/using-volumn-to-persist-data-in-container-a3640cc92ce4

Docker容器創建sql server資料庫時，只要容器不刪除數據會一直保留
**容器重啟後資料會保留**：當你只是停止容器（`docker stop`）並且之後重啟（`docker start`）時，資料會保留在容器內的文件系統中。
**刪除容器後資料會丟失**：如果你刪除容器（`docker rm`），容器內的所有數據，包括資料庫中的資料，都會永久丟失，因為 Docker 容器的文件系統是臨時性的，隨著容器的刪除而清空。

因為`Evaluation`只能保存180天的連線，在試用期過後則不能進入sql server內
- 解決方案 : 使用 `Docker Volumes`將數據保留以防數據遺失

![](https://i.imgur.com/ObTgbBi.png)


- 創建儲存器 `sqlservervol`
```cmd
docker volume create sqlservervol
```
- 查看儲存器
```cmd
docker volume list
```