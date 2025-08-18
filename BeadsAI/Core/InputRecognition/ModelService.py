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

def main(args):
    
    model_service = service(args)

    model_service.listen()

class service:

    def __init__(self,args):

        self.handle_args(args)

        self.model = model(self.modelpath,self.tmpimgpath)

    def handle_args(self,args):

        if len(args) != 3:
            service.say(f"python: argument length '{len(args)}' expected 3.")
            sys.exit(1)

        self.modelpath = args[1]
        self.tmpimgpath = args[2]

        if not os.path.exists(self.modelpath):
            service.say(f"python: {self.modelpath} is not a valid path for model.")
            sys.exit(1)

        if not os.path.exists(self.tmpimgpath):
            service.say(f"python: {self.tmpimgpath} is not a valid path for image.")
            sys.exit(1)

    def listen(self):

        for input in sys.stdin:
            command = input.strip()
            responce = self.switch(command)

            self.say(responce)

    @staticmethod
    def say(output):

        print(output)
        sys.stdout.flush()

    def switch(self,command):

        if command == "Load":
            self.model.load()
            responce = "Success"

        elif command == "Classify":
            responce = self.model.classify()

        else:
            responce = "python: Unidentified command"

        return responce 

class model:

    def __init__(self,modelpath,tmpimgpath):
        
        self.modelpath = modelpath
        self.tmpimgpath = tmpimgpath
        self.image_size = (224,224)
        self.data = np.ndarray(shape=(1, 224, 224, 3), dtype=np.float32) # fine?
        self.loaded = False

    def load(self):

        if self.loaded:
            return

        self.model = load_model(self.modelpath,compile=False)
        self.loaded = True

    def classify(self):

        self.load()

        image = Image.open(self.tmpimgpath).convert("RGB")
        image = ImageOps.fit(image, self.image_size, Image.Resampling.LANCZOS)
        
        image_array = np.asarray(image)

        normalized_image_array = (image_array.astype(np.float32) / 127.5) - 1

        self.data[0] = normalized_image_array

        prediction = self.model.predict(self.data,verbose=0)
        index = np.argmax(prediction)

        image.close()

        return index

if __name__ == "__main__":
   main(sys.argv)

'''
test = model("C:\\BeadsFolder\\Model\\Model.h5","C:\\BeadsFolder\\tmpfile.jpg")
test.load()
print(test.classify())
print(test.classify())
'''