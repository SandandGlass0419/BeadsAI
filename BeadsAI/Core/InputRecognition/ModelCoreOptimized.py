# Run on Python 3.11.7, tensorflow 2.12.1 to avoid errors pywin32
from keras.models import load_model
from PIL import Image, ImageOps
import tensorflow as tf
import numpy as np
import win32pipe
import win32file
import sys
import os
import json
import base64

tf.get_logger().setLevel('ERROR')
os.environ['TF_CPP_MIN_LOG_LEVEL'] = '3'
np.set_printoptions(suppress=True)

class modelserver:

    def __init__(self,modelpath):
        self.model = load_model(modelpath,compile=False)
        self.pipe_name = r'\\.\pipe\ModelCorePipe' # server name, pipe name
        self.pipe = None

    def connect(self):
        self.pipe = win32pipe.CreateNamedPipe(
            self.pipe_name,
            win32pipe.PIPE_ACCESS_DUPLEX,
            win32pipe.PIPE_TYPE_MESSAGE | win32pipe.PIPE_READMODE_MESSAGE | win32pipe.PIPE_WAIT,
            1, 65536, 65536, 0, None
        )

        win32pipe.ConnectNamedPipe(self.pipe,None)

    def send_response(self,response):
        win32file.WriteFile(self.pipe, json.dumps(response).encode('utf-8'))

    def run_server(self):
        self.connect()

        while True:
            data = win32file.ReadFile(self.pipe, 65536)
            request = json.loads(data.decode('utf-8'))

            if request['command'] == 'predict':
                prediction = self.classify(request['imagepath'])
                response = {'status': 'success', 'prediction': prediction}

            elif request['command'] == 'exit':
                response = {'status': 'exit'}
                self.send_response(response)
                break

            else:
                response = {'status': 'error'}
            
            self.send_response(response)
        
        if self.pipe:
            win32file.CloseHandle(self.pipe)

    def classify(self,imagepath):
        data = np.ndarray(shape=(1, 224, 224, 3), dtype=np.float32)

        image = Image.open(imagepath).convert("RGB")

        size = (224, 224)
        image = ImageOps.fit(image, size, Image.Resampling.LANCZOS)

        image_array = np.asarray(image)

        normalized_image_array = (image_array.astype(np.float32) / 127.5) - 1

        data[0] = normalized_image_array

        prediction = self.model.predict(data,verbose=0)
        index = np.argmax(prediction)

if __name__ == "__main__":
    if len(sys.argv) != 2:
        print("Error: Use python ModelCore.py <modelpath>")
        sys.exit(1)

    modelpath = sys.argv[1]

    server = modelserver(modelpath)

'''
    if not os.path.exists(modelpath):
        print(f"Error: {modelpath} is not a valid path for model.")
        sys.exit(1)

    if not os.path.exists(imagepath):
        print(f"Error: {imagepath} is not a valid path for image.")
        sys.exit(1)
'''