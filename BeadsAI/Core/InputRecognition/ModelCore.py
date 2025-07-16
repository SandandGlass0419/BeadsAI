# Run on Python 3.11.7, tensorflow 2.12.1 to avoid errors pywin32
from keras.models import load_model
from PIL import Image, ImageOps
import tensorflow as tf
import numpy as np
import sys
import os

tf.get_logger().setLevel('ERROR')
os.environ['TF_CPP_MIN_LOG_LEVEL'] = '3'
np.set_printoptions(suppress=True)

def Classify(modelpath,imagepath):
    model = load_model(modelpath,compile=False)

    data = np.ndarray(shape=(1, 224, 224, 3), dtype=np.float32)

    image = Image.open(imagepath).convert("RGB")

    size = (224, 224)
    image = ImageOps.fit(image, size, Image.Resampling.LANCZOS)

    image_array = np.asarray(image)

    normalized_image_array = (image_array.astype(np.float32) / 127.5) - 1

    data[0] = normalized_image_array

    prediction = model.predict(data,verbose=0)
    index = np.argmax(prediction)
    confidence_score = prediction[0][index]

    print(index) # using stdout to feed result

if __name__ == "__main__":
    if len(sys.argv) != 3:
        print("Error: Use, python ModelCore.py <modelpath> <imagepath>")
        sys.exit(1)

    imagepath = sys.argv[1]
    modelpath = sys.argv[2]

    if not os.path.exists(modelpath):
        print(f"Error: {modelpath} is not a valid path for model.")
        sys.exit(1)

    if not os.path.exists(imagepath):
        print(f"Error: {imagepath} is not a valid path for image.")
        sys.exit(1)

    Classify(modelpath,imagepath)