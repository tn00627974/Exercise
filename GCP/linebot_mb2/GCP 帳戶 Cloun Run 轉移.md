- 20240714　轉移
- A帳戶為 tn00627973
- B帳戶為 tn00627972

- 20241012　轉移
- A帳戶為 tn00627972 : 專案 `line0714`
- B帳戶為 tn00627971 : 專案 `linebot-20241010`
- Docker建置LINEBOT`aiface`  跟模型`mb220241010`

- 20241229　轉移
- A帳戶為 tn00627971 : 專案 `linebot-20241010```
- B帳戶為 tn00627999 : 專案 `linebot-20241229`
- Docker建置LINEBOT`aiface`  跟模型`mb220241229`



=== 主要步驟如下 ===
1.[[#A 帳戶 轉移 Cloun Run 到 B帳戶==]]
2.切換至A帳戶使用 Docker 指令 : `開啟拉取指令`>`執行pull` >`建立tag A to B`
3.建立B帳戶專案的`Artifact Registry 儲存庫`
4.在B帳戶將
3.切換至B帳戶專案 > `部署映像檔到 Cloud Run`
## ==A 帳戶 轉移 Cloun Run 到 B帳戶==

- A進入IAM  授權使用 給B帳號 可參考 [[Ulysses/成長筆記本/資料工程師/雲端部屬/GCP/GCP VM 帳戶 映像檔轉移|GCP VM 帳戶 映像檔轉移]]

## ==A帳戶的 linebot-deploy 拉取==
從**A**專案(tn00627971 帳號) `Artifact Registry` -> 進入`aiface20241010` 的 `aiface`與 `mb220241010` 的 `mobilenetv2`點選進入後可看到 -> 顯示提取指令

![](https://i.imgur.com/04qjJJr.png)

![](https://i.imgur.com/30l6zxl.png)
### 1. **B帳號**執行指令，讓Docker 可以拉取映像檔``
```
gcloud auth configure-docker us-central1-docker.pkg.dev
```
將這些命令與 Docker 用戶端一起使用來拉取映像。若要使用這些指令，您的 Docker 用戶端必須配置為透過 us-central1-docker.pkg.dev 進行身份驗證。如果這是您第一次使用 Docker 用戶端從 us-central1-docker.pkg.dev 提取映像，請在安裝了 Docker 的電腦上執行下列命令。

### 2.開始使用pull 提取映像檔

```cloud shell
docker pull \
    us-central1-docker.pkg.dev/linebot-20241010/aiface20241010/aiface:v1
```

###  3.拉取之後可以使用 `docker images`查看映像檔

###  4.接著我們切換至 **B帳號** 使用 Clund Shell 去執行拉取 **A帳號** 的 `images` 以下 :

*20241012* Artifact Registry `aiface20241010`
```
docker pull \ us-central1-docker.pkg.dev/linebot0714/linebot/aiface:v1
```
*20241012* Artifact Registry`mb220241010`
```
docker pull \ us-central1-docker.pkg.dev/linebot0714/mb2/mobilenetv2:latest
```


*20241229* Artifact Registry`aiface20241229`
```cloud shell
docker pull \us-central1-docker.pkg.dev/linebot-20241010/aiface20241010/aiface:v1
```
*20241229* Artifact Registry`mb220241229`
```cloud shell
docker pull \us-central1-docker.pkg.dev/linebot-20241010/mb220241010/mobilenetv2:latest
```
## ==B帳戶的 linebot-deploy 建立標籤映像檔==

*20240714*
- 若已經有Tag標籤了，可以直接Push不用在標籤一次  
- `docker tag` 命令需要兩個參數：來源映像檔（SOURCE_IMAGE）和目標映像檔（TARGET_IMAGE）
```cloun shell
tn00627972@cloudshell:~ (linebot-deploy)$ docker tag us-central1-docker.pkg.dev/linebot-deploy/linebot/aiface:v1    
```

*20241012* - tag `aiface20241010`
```cloun shell
docker tag us-central1-docker.pkg.dev/linebot0714/linebot/aiface:v1 us-central1-docker.pkg.dev/linebot-20241012/aiface20241012/aiface:v1
```
*20241012* - tag `mb220241010`
```cloun shell
docker tag us-central1-docker.pkg.dev/linebot0714/mb2/mobilenetv2:latest us-central1-docker.pkg.dev/linebot-20241010/mb220241010/mobilenetv2:latest
```

*20241229* - tag `aiface20241229`
```cloun shell
docker tag us-central1-docker.pkg.dev/linebot-20241010/aiface20241010/aiface:v1 \
us-central1-docker.pkg.dev/linebot-20241229/aiface20241229/aiface:v1
```

*20241229* - tag `mb220241229`
```cloun shell
docker tag us-central1-docker.pkg.dev/linebot-20241010/mb220241010/mobilenetv2:latest \
us-central1-docker.pkg.dev/linebot-20241229/mb220241229/mobilenetv2:latest
```
## ==創建 B帳戶 Artifact Registry 儲存庫==

1. **在 Google Cloud Console 中創建儲存庫**：
    
    - 登入 Google Cloud Console。
    - 導航到 "Artifact Registry"。
    - 點擊 "Create Repository"。
    - 填寫儲存庫的名稱（如 `linebot`），選擇地區（如 `us-central1`），並選擇儲存庫格式為 `Docker`。
    - 點擊 "Create"。


## ==推送映像檔到 B帳戶的 Artifact Registry==

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


*20241229* - push `aiface20241229`
```
docker push us-central1-docker.pkg.dev/linebot-20241229/aiface20241229/aiface:v1
```

*20241229* - push `mb220241229`
```
docker push us-central1-docker.pkg.dev/linebot-20241229/mb220241229/mobilenetv2:latest
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

說明：
- `mb2-service`: 這是你的Cloud Run 服務名稱。
- `--image`: 指定你要部署的 Docker 映像。
- `--platform managed`: 使用 Google Cloud Run 的受管理平台。
- `--region us-central1`: 指定部署的區域，這裡用的是 `us-central1`。
	(若沒加入參數的話要選32)

![[Ulysses/成長筆記本/資料工程師/附件圖片檔/Pasted image 20240714134011.png]]

![[Ulysses/成長筆記本/資料工程師/附件圖片檔/Pasted image 20241012220439.png]]
- 選 `[32] us-central1` 

*20241012*
- 切換到要佈署專案後，開始進行佈署
```cloun shell
gcloud run deploy aiface-service 
--image us-central1-docker.pkg.dev/linebot-20241010/aiface20241010/aiface:v1 
--platform managed 
--region us-central1
```

*20241012*
```
gcloud run deploy mb2-service 
--image us-central1-docker.pkg.dev/linebot-20241010/mb220241010/mobilenetv2:latest 
--platform managed 
--region us-central1
```

*20241229*
- 切換到要佈署專案後，開始進行佈署
```cloun shell
gcloud run deploy aiface-services \
  --image us-central1-docker.pkg.dev/linebot-20241229/aiface20241229/aiface:v1 \
  --platform managed \
  --region us-central1
```

*20241229*
```
gcloud run deploy mb2-services \
--image us-central1-docker.pkg.dev/linebot-20241229/mb220241229/mobilenetv2:latest \
--platform managed \
--region us-central1
```
##### Clund shell 小知識
- 檢查已有的 Artifact Registry 儲存庫：
```clund shell
gcloud artifacts repositories list --project=<目標專案ID> --format="table(name)"
```

```clund shell
gcloud artifacts repositories list --project=linebot-20241229 --format="table(name)"
```

