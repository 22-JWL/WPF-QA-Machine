코드 설명

   1. Model (`Part.cs`): Part 클래스는 개별 부품의 데이터(ID, 길이, 무게, 상태)를 저장하는 간단한 C# 클래스입니다.

   2. MVVM Helpers (`ViewModelBase.cs`, `RelayCommand.cs`):
       * ViewModelBase: INotifyPropertyChanged 인터페이스를 구현하여, ViewModel의 속성이 변경될 때 UI가 자동으로 업데이트될 수 있도록 합니다.
       * RelayCommand: ICommand 인터페이스를 구현하여, UI의 버튼 클릭과 같은 이벤트를 ViewModel의 메서드에 연결(바인딩)할 수 있게 해줍니다.

   3. ViewModel (`MainViewModel.cs`):
       * UI와 상호작용하는 모든 로직을 포함합니다.
       * CurrentPart: 현재 검사 중인 부품의 정보를 UI에 보여주기 위한 속성입니다.
       * ClassifiedParts: 검사가 완료된 모든 부품의 목록을 저장하며, ObservableCollection으로 만들어져 목록에 변경이 생기면 UI의 리스트가 자동으로 업데이트됩니다.
       * InspectNextPartCommand: "다음 부품 검사" 버튼이 눌렸을 때 실행될 InspectNextPart 메서드를 연결합니다.
       * InspectNextPart 메서드:
           * 정상 범위를 벗어나는 값을 가질 수 있는 더미 부품 데이터를 랜덤하게 생성합니다.
           * 미리 정의된 기준(길이 9.9\~10.1, 무게 4.9\~5.1)에 따라 부품을 "정상" 또는 "불량"으로 판별합니다.
           * 결과를 CurrentPart에 업데이트하고 ClassifiedParts 목록에 추가합니다.

   4. View (`MainWindow.xaml`):
       * 사용자에게 보여지는 UI입니다. XAML로 작성되었습니다.
       * <Window.DataContext>를 통해 View를 MainViewModel에 연결합니다.
       * TextBlock과 같은 컨트롤의 Text 속성을 {Binding CurrentPart.PartId}와 같이 ViewModel의 속성에 바인딩하여 데이터를 표시합니다.
       * Button의 Command 속성을 {Binding InspectNextPartCommand}로 바인딩하여 버튼 클릭 시 ViewModel의 로직이 실행되도록 합니다.
       * DataTrigger를 사용하여 부품의 Status 값("정상" 또는 "불량")에 따라 글자색이 녹색 또는 빨간색으로 자동 변경되도록 스타일을 적용했습니다.

   5. View Code-behind (`MainWindow.xaml.cs`):
       * 이번 예제에서는 XAML에서 DataContext를 설정했기 때문에, 코드 비하인드 파일은 거의 비어있습니다. 이것은 MVVM 패턴의 장점 중 하나로, UI의 모양(View)과 로직(ViewModel)이
         깔끔하게 분리됩니다.

변경 사항 요약

   1. `MainViewModel.cs`:
       * 부품의 정상/불량 판별 기준(최소/최대 길이, 무게)이 이제 하드코딩된 값이 아닌, 언제든지 변경할 수 있는 속성(MinLength, MaxLength 등)이 되었습니다.
       * 사용자의 명령어를 처리하기 위한 ExecuteUserCommand가 추가되었습니다.
       * ProcessUserCommand 메서드는 사용자가 입력한 텍스트를 분석하여 상태 확인, 기준 변경, 초기화, 도움말 보기 등의 동작을 수행합니다.
       * 모든 명령어 처리 결과와 시스템 메시지는 CommandLog 속성에 기록되어 UI에 표시됩니다.

   2. `MainWindow.xaml`:
       * 메인 화면이 Grid를 통해 좌우 두 영역으로 나뉘었고, 중간에 GridSplitter를 두어 사용자가 크기를 조절할 수 있게 했습니다.
       * 왼쪽: 기존의 부품 검사 및 분류 기록 UI가 위치합니다.
       * 오른쪽: 새로 추가된 명령어 인터페이스가 위치합니다.
           * 상단에 현재 적용되고 있는 정상 기준 범위를 표시하여 사용자가 언제든 확인할 수 있습니다.
           * 중앙에는 읽기 전용 TextBox를 두어 CommandLog의 내용을 보여줍니다.
           * 하단에는 사용자가 명령어를 입력할 TextBox와 "전송" Button이 있습니다. 엔터 키를 눌러도 명령이 실행되도록 KeyBinding을 추가했습니다
