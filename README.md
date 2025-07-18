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

***Core\InputRecognition\ModelCore.py:***  
이미지를 받아 keras 모델을 통해 결과값을 받는다.  

***Core\InputRecognition\ModelCoreOptimized.py:***  
위와 같은 기능이나, 모델을 초기에 한번만 로딩해 속도가 더 빠르다. 미완성 상태이다.  

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

***UserControls\BraceletDisplayControl.xaml(cs):***  
UI 중 팔찌의 각 색을 표시하는 부분을 담당한다.

***UserControl\ColorSelectWindow.xaml(cs):***  
UI 중 BraceletDisplayControl 에서 비즈 색을 바꾸기 위해 버튼을 누르면 보이는 색 변경창 부분을 담당한다.  

***UserControl\InputRecognition.cs:***  
InputSelectControl 중 Camera 탭에서 카메라에 빛친 비트맵을 전달받아 파일화 시킨 후 ModelCore.py 를 통해 입력을 분석한다.  

***UserControl\InputSelectControl.xaml(cs):***  
UI 중 신경망의 입력을 선택하는 부분을 담당한다.  

***UserControl\MessageBus.cs:***  
각 UI 간에 정보를 전달하는 역할을 한다.  

***UserControl\NetworkRunControl.xaml(cs):***  
화면 왼쪽 아래에 위치한 Rerun-Test 와 Run 버튼을 가지고 있다.  
UI 내의 자료를 총괄하고, Core\ 을 구성하는 클래스를 소환해 실행한다.  

***UserControl\OutputDisplayControl.xaml(cs):***  
출력을 퍼센트로 나타내는 UI 이다. 화면 오른쪽 큰 부분을 차지하는 UI 이다.  

***UserControl\ToolBarControl.xaml(cs):***  
화면 위 New 와 Logs 버튼을 가진 UI 이다. 새로운 사진을 부르거나(팔찌 사진), 로그 파일을 열 수 있다.  

***UserControl\UIBead.cs:***  
BraceletDisplayControl 에서 사용하는 클래스로 각 비즈의 위치, 색(UI 에서), 문자열 이름을 소유하고 있다.  
















