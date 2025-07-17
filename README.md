Dependancies:
.NET 8, Python 3.11.7 (tensorflow 2.12.1,pillow,numpy)

각 파일/클래스별 기능:  

***Core\ExceptionThrower.cs:***  

**class ExceptionThrower:**
조건에 맞지 않을 경우 오류가 이 클래스를 통해 메시지 박스와 함께 발동된다.

***Core\ColorRecognition\Croptool.cs:***  

**class CropTool:**  
이미지를 작품에 필요한대로 자를수 있게 하는 static 메서드들이 포함돼있다. BraceletRecognition 클래스에서 주로 활용된다.

***Core\ColorRecognition\RGBFindCore.cs:*** 

**interface IRGBFindConfig:**  
RGBFindCore 에 사용되는 설정값들을 형식을 정의한 인터페이스 이다. BraceletRecognition 클래스에서 활용된다.

**class RGBFindCore:**  
이미지를 받아 특정 부분의 색이 정의된 색중 어떤 색과 가장 가까운지 찾는 클래스이다. BraceletRecognition 클래스에서 활용된다.

***Core\NeuralNetwork\ExampleWeights.cs:***  

**class ExampleWeights:**  
각 비즈색에 정의될 가중치 값을 가진 float[] 를 반환하는 메서드들이 포함돼있다. BraceletNetwork 클래스에서 주로 활용한다.

***Core\NeuralNetwork\NetworkConfig.cs:***  

**class NetworkConfig:**  
신경망이 작동하기 위해 입력텐서, 출력텐서, 히든텐서, 입력, 팔찌색 조합 조건을 확인하는 메서드들이 포함되어있다.  
조건에 만족하지 못하는 경우 ExceptionThrower 을 통해 오류가 발생한다.  

**class WeightColorConfig:**  
각 색별로 정의된 가중치의 값들(float[]) 이 조건에 부합하는지 확인하는 클래스이다.  

***Core\NeuralNetwork\NetworkCore.cs:***  

**class WeightColor:**  
각 색의 문자열 표현과 가중치의 값들을 나타내는 클래스이다. 신경망 구현에서 내부적으로 사용한다.  

**class NetworkCore:**
조건에 따라 입력, 히든, 출력 텐서를 만들고, List 에 저장해 신경망 모델을 구현한다.  
TorchSharp 패키지를 사용해 구현하였다.













