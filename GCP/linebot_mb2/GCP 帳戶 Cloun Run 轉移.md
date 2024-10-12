- 20240714　轉移
- A帳戶為 tn00627973
- B帳戶為 tn00627972

- 20241012　轉移
- A帳戶為 tn00627972 : 專案 `line0714`
- B帳戶為 tn00627971 : 專案 `linebot-20241010`
- Docker建置LINEBOT`aiface`  跟模型`mb220241010`


=== 主要步驟如下 ===
1.[[#==A 帳戶 轉移 Cloun Run 到 B帳戶==]]
2.切換至A帳戶使用 Docker 指令 : `開啟拉取指令`>`執行pull` >`建立tag A to B`
3.建立B帳戶專案的`Artifact Registry 儲存庫`
4.在B帳戶將
3.切換至B帳戶專案 > `部署映像檔到 Cloud Run`
## ==A 帳戶 轉移 Cloun Run 到 B帳戶==

- A進入IAM  授權使用 給B帳號 可參考 [[Ulysses/成長筆記本/資料工程師/雲端部屬/GCP/GCP VM 帳戶 映像檔轉移|GCP VM 帳戶 映像檔轉移]]


## ==執行指令，讓Docker 可以拉取映像檔==
```
gcloud auth configure-docker us-central1-docker.pkg.dev
```
將這些命令與 Docker 用戶端一起使用來拉取映像。若要使用這些指令，您的 Docker 用戶端必須配置為透過 us-central1-docker.pkg.dev 進行身份驗證。如果這是您第一次使用 Docker 用戶端從 us-central1-docker.pkg.dev 提取映像，請在安裝了 Docker 的電腦上執行下列命令。


## ==A帳戶的 linebot-deploy 拉取==
*20241012*`linebot`
- Pull
```cloun shell
docker pull \ us-central1-docker.pkg.dev/linebot-deploy/linebot/aiface:v1
```
- 拉取後可以Docker指令查看 images
```Docker 
docker images
```

![[Ulysses/成長筆記本/資料工程師/附件圖片檔/Pasted image 20240714140151.png]]

*20241012*`aiface20241010`
```
docker pull \ us-central1-docker.pkg.dev/linebot0714/linebot/aiface:v1
```
*20241012*`mb220241010`
```
docker pull \ us-central1-docker.pkg.dev/linebot0714/mb2/mobilenetv2:latest
```


## ==A帳戶的 linebot-deploy 標籤映像檔==

*20240714*
- 若已經有Tag標籤了，可以直接Push不用在標籤一次  
- `docker tag` 命令需要兩個參數：來源映像檔（SOURCE_IMAGE）和目標映像檔（TARGET_IMAGE）
```cloun shell
tn00627972@cloudshell:~ (linebot-deploy)$ docker tag us-central1-docker.pkg.dev/linebot-deploy/linebot/aiface:v1    
```


*20241012* - tag `aiface20241010`
```
docker tag us-central1-docker.pkg.dev/linebot0714/linebot/aiface:v1 us-central1-docker.pkg.dev/linebot-20241012/aiface20241012/aiface:v1
```
*20241012* - tag `mb220241010`
```
docker tag us-central1-docker.pkg.dev/linebot0714/mb2/mobilenetv2:latest us-central1-docker.pkg.dev/linebot-20241010/mb220241010/mobilenetv2:latest
```


## ==創建 Artifact Registry 儲存庫==

1. **在 Google Cloud Console 中創建儲存庫**：
    
    - 登入 Google Cloud Console。
    - 導航到 "Artifact Registry"。
    - 點擊 "Create Repository"。
    - 填寫儲存庫的名稱（如 `linebot`），選擇地區（如 `us-central1`），並選擇儲存庫格式為 `Docker`。
    - 點擊 "Create"。


## ==推送映像檔到 B 帳戶的 Artifact Registry==

Artifact Registry先建立 `linebot` 資料夾
- Push
```cloun shell
docker push us-central1-docker.pkg.dev/linebot0714/linebot/aiface:v1
```

*20241012* - push `aiface20241010`
```
docker push us-central1-docker.pkg.dev/linebot-20241010/aiface20241010/aiface:v1
```

*20241012* - push `mb220241010`
```
docker push us-central1-docker.pkg.dev/linebot-20241010/mb220241010/mobilenetv2:latest
```



## ==切換到要佈署的專案==

切換到`tn00627972`的 `linebot0714`
```cloun run
gcloud config set project linebot0714
```

![[Ulysses/成長筆記本/資料工程師/附件圖片檔/Pasted image 20240714133613.png]]

切換到`tn00627972`的 `linebot-20241010`
```
gcloud config set project linebot-20241010
```

## ==部署映像檔到 Cloud Run==

*20240714*
- 切換到要佈署專案後，開始進行佈署
```cloun shell
gcloud run deploy aiface-service --image us-central1-docker.pkg.dev/linebot0714/linebot/aiface:v1 --platform managed
```

![[Ulysses/成長筆記本/資料工程師/附件圖片檔/Pasted image 20240714134011.png]]

![[Ulysses/成長筆記本/資料工程師/附件圖片檔/Pasted image 20241012220439.png]]
- 選 `[32] us-central1` 
*20241012*
- 切換到要佈署專案後，開始進行佈署
```cloun shell
gcloud run deploy mb2-service us-central1-docker.pkg.dev/linebot-20241010/mb220241010/mobilenetv2:latest --platform managed
```

*20241012*
```
gcloud run deploy mb2-service us-central1-docker.pkg.dev/linebot-20241010/mb220241010/mobilenetv2:latest --platform managed
```
