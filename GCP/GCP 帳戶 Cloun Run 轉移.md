- A帳戶為 tn00627973
- B帳戶為 tn00627972

## ==A 帳戶 轉移 Cloun Run 到 B帳戶==

- A進入IAM  授權使用 給B帳號


## ==執行指令，讓Docker 可以拉取映像檔==
```
gcloud auth configure-docker us-central1-docker.pkg.dev
```
將這些命令與 Docker 用戶端一起使用來拉取映像。若要使用這些指令，您的 Docker 用戶端必須配置為透過 us-central1-docker.pkg.dev 進行身份驗證。如果這是您第一次使用 Docker 用戶端從 us-central1-docker.pkg.dev 提取映像，請在安裝了 Docker 的電腦上執行下列命令。


## ==A帳戶的 linebot-deploy 拉取==
- Pull
```cloun shell
docker pull \ us-central1-docker.pkg.dev/linebot-deploy/linebot/aiface:v1 v1: Pulling from linebot-deploy/linebot/aiface
```
- 拉取後可以Docker指令查看 images
```Docker 
docker images
```

![[Ulysses/成長筆記本/資料工程師/附件圖片檔/Pasted image 20240714140151.png]]


### A帳戶的 linebot-deploy 標籤映像檔==
- 若已經有Tag標籤了，可以直接Push不用在標籤一次  
- `docker tag` 命令需要兩個參數：來源映像檔（SOURCE_IMAGE）和目標映像檔（TARGET_IMAGE）
```cloun shell
tn00627972@cloudshell:~ (linebot-deploy)$ docker tag us-central1-docker.pkg.dev/linebot-deploy/linebot/aiface:v1    
```


### ==創建 Artifact Registry 儲存庫==

1. **在 Google Cloud Console 中創建儲存庫**：
    
    - 登入 Google Cloud Console。
    - 導航到 "Artifact Registry"。
    - 點擊 "Create Repository"。
    - 填寫儲存庫的名稱（如 `linebot`），選擇地區（如 `us-central1`），並選擇儲存庫格式為 `Docker`。
    - 點擊 "Create"。


### ==推送映像檔到 B 帳戶的 Artifact Registry==

Artifact Registry先建立 `linebot` 資料夾
- Push
```cloun shell
docker push us-central1-docker.pkg.dev/linebot0714/linebot/aiface:v1
```


### ==切換到要佈署的專案==
切換到`tn00627972`的 `linebot0714`
```cloun run
gcloud config set project linebot0714
```

![[Ulysses/成長筆記本/資料工程師/附件圖片檔/Pasted image 20240714133613.png]]

### ==部署映像檔到 Cloud Run==

- 切換到要佈署專案後，開始進行佈署
```cloun shell
gcloud run deploy aiface-service --image us-central1-docker.pkg.dev/linebot0714/linebot/aiface:v1 --platform managed
```

![[Ulysses/成長筆記本/資料工程師/附件圖片檔/Pasted image 20240714134011.png]]