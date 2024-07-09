# SYSnet Android
Unity Project for SYStronics SYSnet Remote Monitoring App

## Android Unity Plugin
- https://github.com/sgho0915/SYSnetA_UnityPlugin
- Wi-fi, Ethernet 상태 확인
- Storage 상태 확인
- 앱 내 설치
- 이미지 선택
- jwsapi GPIO 제어

## 사용 기술
- Unity 2022.3.22 LTS
- Android (Min API Level 5.1 Lollipop(API 22), Target API Level(Automatic(highest installed)))
- MySqlConnector 2.2.7 (Connect For Database)
- Newtonsoft.Json 13.0.3 (Connect For JsonData)

## 최적화 기술
- ObjectPool 사용을 통한 오브젝트 생성 및 관리 최적화
- UnityMainThreadDispatcher 사용을 통한 데이터 통신 스레드 및 UI 스레드 분리(https://github.com/PimDeWitte/UnityMainThreadDispatcher)
- StringBuilder, 오브젝트 캐싱 사용

## 사용 에셋 & 플러그인
1. Realtime In Game Log Console (https://assetstore.unity.com/packages/tools/gui/realtime-in-game-log-console-212758#reviews)
  - 디바이스 로그 확인용도
  - Asset/Desdinova/RealtimeInGameLogConsole/Scripts/ConsoleGUIController.cs
  - 로그를 확인하고자 하는 Scene에 Empty GameObject 생성 후 ConsoleGUIController.cs 컴포넌트 삽입 후 확인
2. Unity Lottie Animation Plugin(https://github.com/gindemit/unity-rlottie)
  - Lottie Files Json 애니메이션 적용 플러그인
3. E2Chart (https://assetstore.unity.com/packages/tools/gui/e2chart-147893)
  - 트렌드 데이터 그래프 작성 용도
4. DatePicker for UnityUI (https://assetstore.unity.com/packages/tools/gui/datepicker-for-unityui-68264)
  - 날짜 선택
5. Dotween
6. openweathermap 날씨 API
7. gree Unity Web View(https://github.com/gree/unity-webview/tree/master)


## 문제 해결 히스토리
1. HTTP 링크 연결 불가
  - AndroidManifest 파일의 application 태그에  android:usesCleartextTraffic="true" 추가 및 manifest 태그 하위에 <uses-permission android:name="android.permission.INTERNET" /> 권한 추가
  - Unity Editor에서 확인 시 Edit - Project Settings - Player - Other Settings - Allow downloads over HTTP - Always allowed 선택

2. AndroidManifest 커스텀
  - File - Build Settings - Export Project 체크하여 src 폴더에 생성된 AndroidManifest.xml 파일을 Assets/Plugins/Android에 붙여넣기
  - Project Settings - Player - Publishing Settings - Custom Main Manifest 체크

3. 앱 OTA 업데이트
  - https://heon-dev.tistory.com/12
  - https://m.blog.naver.com/cdw0424/222965594170
    
4. WIFI 상태 관련
  - https://stackoverflow.com/questions/13932724/getting-wifi-signal-strength-in-android
  - https://forum.unity.com/threads/android-wifi-manager.1426377/

5. 평면도 관련 스크롤뷰 PinchableScrollRect
  - https://github.com/LokoSoloGames/Unity_PinchableScrollRect
  - https://openupm.com/packages/com.lokosolo.pinchable-scrollrect/

## 프로젝트 깃허브 관리 유의사항 
1. Unity 버전 통일
  - .meta 파일 내용이 유니티 에디터 버전에 따라 달라질 수 있으므로 버전 통일은 매우 중요
  - 버전이 다를 경우 메타파일들이 계속 불필요하게 변경 및 커밋 될 수 있음

2. Asset 직렬화 방식
  - Unity 엔진에서 에셋의 파일을 저장하는 방법은 Binary, Text, 두 가지를 섞어 사용하는 방식이 존재
  - 최신 버전의 경우 기본적으로 Text를 사용하도록 되어있으나, Binary 설정된 구성원이 있다면 관리가 어렵고 충동 발생 시 merge 과정이 매우 복잡함
  - 따라서 Asset 직렬화 방식은 Text로 통일하도록 한다.
  - 이를 바꾸기 위해 Unity 상단 메뉴바에서 Edit - Project Settings - Editor - Asset Serialization Mode - Force Text를 선택해준다.

3. Version Control
  - Unity 엔진에서 메타파일에 기록된 내용을 바탕으로 Scene에 포함된 오브젝트들을 관리한다.
  - 하지만 Version Control Mode가 구성원마다 다를 경우, 구성원 간 풀 및 푸시를 할 때 문제가 발생할 수 있다.
  - 따라서 별도의 달 프로그램을 사용하는 것이 아니라면 Edit - Project Settings - Version Control - Version Control - Mode - Visible Meta Files 로 통일하여 사용하는 것을 권장

4. 새로운 스크립트 생성 시
  - 해당 스크립트를 띄운채로 VisualStudio 파일 - 다른 이름으로 "스크립트명.cs" 저장 - 저장 버튼 옆의 화살표 - 인코딩하여 저장 - 확인 - 인코딩 형식 "유니코드(서명 있는 UTF-8) - 코드 페이지 65001" 선택
   -
