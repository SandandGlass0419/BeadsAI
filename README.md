Dependancies:
.NET 8, Python 3.11.7 (tensorflow 2.12.1,pillow,numpy)

각 파일/클래스별 기능:  

***Core\ExceptionThrower.cs***  

**class ExceptionThrower**  
조건에 맞지 않을 경우 오류가 이 클래스를 통해 메시지 박스와 함께 발동된다.

***Core\ColorRecognition\Croptool.cs***  

**class CropTool**  
이미지를 작품에 필요한대로 자를수 있게 하는 static 메서드들이 포함돼있다. BraceletRecognition 클래스에서 주로 활용된다.

***Core\ColorRecognition\RGBFindCore.cs*** 

**interface IRGBFindConfig**  
RGBFindCore 에 사용되는 설정값들을 형식을 정의한 인터페이스 이다. BraceletRecognition 클래스에서 활용된다.

**class RGBFindCore**  
이미지를 받아 특정 부분의 색이 정의된 색중 어떤 색과 가장 가까운지 찾는 클래스이다. BraceletRecognition 클래스에서 활용된다.

***Core\InputRecognition\ModelCore.py***  
이미지를 받아 keras 모델을 통해 결과값을 받는다.  

***Core\InputRecognition\ModelCoreOptimized.py***  
위와 같은 기능이나, 모델을 초기에 한번만 로딩해 속도가 더 빠르다. 미완성 상태이다.  

***Core\NeuralNetwork\ExampleWeights.cs***  

**class ExampleWeights**  
각 비즈색에 정의될 가중치 값을 가진 float[] 를 반환하는 메서드들이 포함돼있다. BraceletNetwork 클래스에서 주로 활용한다.

***Core\NeuralNetwork\NetworkConfig.cs***  

**class NetworkConfig**  
신경망이 작동하기 위해 입력텐서, 출력텐서, 히든텐서, 입력, 팔찌색 조합 조건을 확인하는 메서드들이 포함되어있다.  
조건에 만족하지 못하는 경우 ExceptionThrower 을 통해 오류가 발생한다.  

**class WeightColorConfig**  
각 색별로 정의된 가중치의 값들(float[]) 이 조건에 부합하는지 확인하는 클래스이다.  

***Core\NeuralNetwork\NetworkCore.cs***  

**class WeightColor**  
각 색의 문자열 표현과 가중치의 값들을 나타내는 클래스이다. 신경망 구현에서 내부적으로 사용한다.  

**class NetworkCore**
조건에 따라 입력, 히든, 출력 텐서를 만들고, List 에 저장해 신경망 모델을 구현한다.  
TorchSharp 패키지를 사용해 구현하였다.

***UserControls\BraceletDisplayControl.xaml(cs)***  
UI 중 팔찌의 각 색을 표시하는 부분을 담당한다.

***UserControl\ColorSelectWindow.xaml(cs)***  
UI 중 BraceletDisplayControl 에서 비즈 색을 바꾸기 위해 버튼을 누르면 보이는 색 변경창 부분을 담당한다.  

***UserControl\InputRecognition.cs***  
InputSelectControl 중 Camera 탭에서 카메라에 빛친 비트맵을 전달받아 파일화 시킨 후 ModelCore.py 를 통해 입력을 분석한다.  

***UserControl\InputSelectControl.xaml(cs)***  
UI 중 신경망의 입력을 선택하는 부분을 담당한다.  

***UserControl\MessageBus.cs***  
각 UI 간에 정보를 전달하는 역할을 한다.  

***UserControl\NetworkRunControl.xaml(cs)***  
화면 왼쪽 아래에 위치한 Rerun-Test 와 Run 버튼을 가지고 있다.  
UI 내의 자료를 총괄하고, Core\ 을 구성하는 클래스를 소환해 실행한다.  

***UserControl\OutputDisplayControl.xaml(cs)***  
출력을 퍼센트로 나타내는 UI 이다. 화면 오른쪽 큰 부분을 차지하는 UI 이다.  

***UserControl\ToolBarControl.xaml(cs)***  
화면 위 New 와 Logs 버튼을 가진 UI 이다. 새로운 사진을 부르거나(팔찌 사진), 로그 파일을 열 수 있다.  

***UserControl\UIBead.cs***  
BraceletDisplayControl 에서 사용하는 클래스로 각 비즈의 위치, 색(UI 에서), 문자열 이름을 소유하고 있다.  

***BraceletNetwork.cs***  

**class BraceletNetwork**  
NenworkConfig 의 자식 클래스로 NetworkConfig 이 가지고 있는 설정값들을 생성자에서 설정한다. 
NetworkCore 의 자식이기도 하므로, NetworkConfig 이 가지고 있는 신경망 구성/실행 함수도 사용하며, 
이 프로젝트에 적합하게 기능들을 조합한 함수들이 포함되어 있다.  

NetworkCore 와 NetworkConfig 이 패키지에서 제공하는 코드이고, BraceletNetwork 의 코드가 패키지 사용자의 코드라고 보아도 된다. 
그러나 이가 실현된 방식은 필요한 요소들을 모두 설정하지 않아도 컴파일이 된다는 헛점이 있다. abstract 를 이용하면 이 부분을 고칠 수 있지만, 
뒤에서 나오는것 처럼, 변수 초기화가 오래 걸리는 딕셔너리(value 가 작동이 오래걸리는 함수로 초기화 되는 경우) 같은 경우는 static 을 사용하지 않으면 사용될때 마다 값을 함수로 불러와야 해서 오래걸린다. abstract 와 static 을 둘 다 쓰면 될 것 같지만, 클래스에서 abstract 필드는 실제로 존재하는 인스턴스 이기 때문에, abstract static 은 모순이 된다. 즉, 사용할 수 없다는 뜻이다. BraceletRecognition 에서는 이 부분을 해결하기 위해 interface IRGBFindConfig 를 사용했는데 (인터페이스는 abstract static 를 사용할 수 있다.), 인터페이스는 클래스의 필수 구성 요소를 나타내는 '계약서' 같은 역할이어서 함수 본문을 정의할 수 없고, 즉 함수 재사용이 어렵다는 뜻이다. BraceletRecognition 같은 경우는 재사용하고자 하는 함수가 없었으나, BraceletNetwork 같은 경우는 ColorMetRequirments 함수를 재사용 해야 하므로, 함수 재사용이 되지 않는 인터페이스는 이 문제를 해결하지 못하는 것이다. 그래서 이러한 구조를 사용한 것이다.  

**class InputColor**  
입력 레이어애서 각 비즈 색의 가중치를 부여한 딕셔너리가 있다. 
InputColor, HiddenColor, OutputColor 셋이 모두 존재하는 이유는 입력 레이어는 길이 25 짜리 텐서 (5x5 비트맵)를 받고, 히든 레이어가 8x4 이기 때문에 각 히든 레이어는 길이 8 짜리 텐서를 받는다. 이런 길이의 차이 때문에 세 레이어의 비즈 색별 가중치를 따로 정의한 것이다.  

**class HiddenColor**  
히든 레이어애서 각 비즈 색의 가중치를 부여한 딕셔너리가 있다.  

**class OutputColor** 
출력 레이어애서 각 비즈 색의 가중치를 부여한 딕셔너리가 있다. 

***MainWindow.xaml(cs)***  
UI가 로딩되는 창 이다. 이 창에 UserControls 폴더에 있는 파일들이 모두 구성되어 있다.  


나머지 파일들은 코드 에디터나 컴파일러가 요구하는 파일들로, 무시하셔도 됩니다.














