import cv2
import numpy as np
from PIL import Image
import requests
from tensorflow.keras.applications import MobileNetV2
from tensorflow.keras.applications.mobilenet_v2 import preprocess_input

img = Image.open('222.jpg')
width, height = img.size

# 转换为numpy数组进行处理
img_np = np.array(img)

# 检查高度和宽度，进行填充
if height > width:
    diff = height - width
    left = diff // 2
    right = diff - left
    padding = ((0, 0), (left, right), (0, 0))
elif width > height:
    diff = width - height
    top = diff // 2
    bottom = diff - top
    padding = ((top, bottom), (0, 0), (0, 0))
else:
    padding = ((0, 0), (0, 0), (0, 0))

padded_image = np.pad(img_np, padding, mode="constant", constant_values=0)

# 调整大小
resized_image = cv2.resize(padded_image, (224, 224))

# img_array = np.array(img)
img_array = np.expand_dims(resized_image, axis=0)  # 创建一个 batch
img_array = preprocess_input(img_array)  # 应用预处理

# 预测
img_array = img_array.astype(np.float32)

r = requests.post(
    " https://mb2-service-1061597379035.us-central1.run.app:443/v1/models/mobilenetv2:predict",
    json={"instances": img_array.tolist()},
)
print(r.content)
predicted_class = np.argmax(r.json()["predictions"][0])
print(predicted_class)
