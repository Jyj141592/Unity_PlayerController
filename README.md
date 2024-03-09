# 개요

플레이어의 상태를 FSM 구조로 구현하여 복잡한 분기문을 사용하지 않고 플레이어의 동작을 구현하기 위해 사용.

<img width="791" alt="스크린샷 2024-03-09 141133" src="https://github.com/Jyj141592/Unity_PlayerController/assets/140074412/dcb4cd0a-602b-4cdb-8ac8-af58d47ccecc">

# 사용법

## 커스텀 노드 생성

- PlayerController 네임스페이스의 PCNode 클래스를 상속하는 것으로 커스텀 노드 작성.

```
using PlayerController;
public class CustomNode : PCNode{
  ...
}
```

- CreateNodeMenu 애트리뷰트를 사용해 노드 생성 메뉴의 경로를 설정할 수 있음. CreateNodeMenu 애트리뷰트를 사용하지 않으면 기본 경로에 생성됨.

```
using PlayerController;
[CreateNodeMenu("Custom/CustomNode")]
public class CustomNode : PCNode{
  ...
}
```

  - 오버라이드 가능한 메서드

    - void Init(Gameobject obj) : 에셋이 포함된 오브젝트가 초기화될 때 호출.
   
    - void OnEnter() : state에 진입할 때 호출됨.
   
    - void OnUpdate() : state에 머무르는 동안 매 프레임 호출됨.
   
    - void OnExit() : state에서 나갈 때 호출됨.
  
## 그래프 작성

1. 에셋 생성

  ![스크린샷 2024-03-09 143338](https://github.com/Jyj141592/Unity_PlayerController/assets/140074412/779ddde5-a3a7-4bb4-bb23-4efce66bbef7)

  
2. 에디터 조작

- 대부분의 조작법은 애니메이터와 비슷함
  
  - 1. 노드 생성

    - 빈 공간에 우클릭 -> New node -> 생성할 노드 선택

  - 2. 노드 연결

    - 노드의 위쪽이 입력 포트, 아래쪽이 출력 포트

    - 출력 포트 -> 입력 포트 방향으로 트랜지션이 생성됨

  - 3. 파라미터 생성, 편집

    - 좌측 + 버튼을 눌러 파라미터 생성(int, float, bool 타입 지원)

    - 파라미터 이름을 더블클릭해 이름을 바꿀 수 있음

  - 4. 노드 데이터 수정

    - 노드를 선택하면 우측에 노드의 public 또는 SerializeField 변수들을 편집할 수 있음

  - 5. 트랜지션 데이터 수정

    - 엣지를 선택하면 해당 트랜지션을 편집할 수 있음

  - 6. Ping Asset, Refresh 버튼

    - Ping Asset 버튼을 클릭하면 현재 편집 중인 에셋의 위치가 프로젝트 창에서 표시됨

    - Refresh 버튼을 클릭하면 창이 새로고침됨

## 컴포넌트

오브젝트에 PlayerControl 스크립트를 컴포넌트로 추가해 만든 에셋을 사용할 수 있음.

  - PlayerControl 메서드

    - int GetIndexOfParameter(string name) : name을 이름으로 갖는 파라미터의 인덱스를 리턴. 파라미터에 접근할 때 이름으로 접근하기보다 인덱스로 접근하는게 효율적임.
   
    - int GetInt(string name) : name을 이름으로 갖는 파라미터의 int 값을 리턴
   
    - int GetInt(int index) : index번째의 파라미터의 int 값을 리턴
   
    - void SetInt(string name, int value) : name을 이름으로 갖는 파라미터의 int값을 설정
   
    - void SetInt(int index, int value) : index번째의 파라미터의 int값을 설정
   
    - float GetFloat(string name) : name을 이름으로 갖는 파라미터의 float값을 설정
   
    - float GetFloat(int index) : index번째의 파라미터의 float값을 리턴
   
    - void SetFloat(string name, float value) : name을 이름으로 갖는 파라미터의 float값을 설정
   
    - void SetFloat(int index, float value) : index번째의 파라미터의 float값을 설정
   
    - bool GetBool(string name) : name을 이름으로 갖는 파라미터의 bool값을 리턴
   
    - bool GetBool(int index) : index번째의 파라미터의 bool값을 리턴
   
    - void SetBool(string name, bool value) : name을 이름으로 갖는 파라미터의 bool값을 설정
   
    - void SetBool(int index, bool value) : index번째의 파라미터의 bool값을 설정
   
    - bool SetTransitionMute(string nodeName, int transitionIndex, bool value) : nodeName을 이름으로 갖는 노드의 transitionIndex번째의 트랜지션의 mute 여부를 설정

## 런타임 디버깅

에디터에서 게임을 실행하는 동안 오브젝트의 현재 state와 파라미터들의 값을 실시간으로 확인할 수 있음.

런타임에 그래프나 파라미터의 값을 변경할 수 있지만 몇몇 기능이 제한됨

<img width="791" alt="스크린샷 2024-03-09 142852" src="https://github.com/Jyj141592/Unity_PlayerController/assets/140074412/01d1c096-e44c-4ef1-8ad0-18e4471bfe74">

<img width="791" alt="스크린샷 2024-03-09 142929" src="https://github.com/Jyj141592/Unity_PlayerController/assets/140074412/bb1fc5e1-1fd4-4727-ab8a-267d2b273238">

<img width="791" alt="스크린샷 2024-03-09 143018" src="https://github.com/Jyj141592/Unity_PlayerController/assets/140074412/569f7ab5-7769-4c5b-b942-8b01b43e546d">

# 확인된 오류

  - condition을 추가할 때 가끔씩 추가한 condition이 보이지 않을 때가 있음. 상단의 refresh 버튼을 누르면 해결
