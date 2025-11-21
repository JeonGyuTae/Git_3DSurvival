# 3D Survival

## 1인칭 생존 어드밴처 게임
<br>

<img width="1036" height="577" alt="5" src="https://github.com/user-attachments/assets/19b91a72-b6a4-419f-9461-d6e28b07ff20" />

## 1. 프로젝트 소개

1인칭 기반 3D 게임프로젝트입니다. 무인도에 표류하여 자원을 수집하고 동물을 사냥해나가며 뗏목을 타고 탈출하는 컨셉입니다.
이 프로젝트에서는 다음과 같은 기능을 구현하였습니다.

 - 캐릭터 이동 구현 및 상호작용
 - 자원 수집 및 가공
 - 적과의 전투
 - NPC와의 대화 기능
 - Map 지도 활성화
 - 식사와 수분 관리


## 2. 팀원 구성 및 역할

|Leader|Team1|Team2|Team3|Team4|
|:-----:|:-----:|:-----:|:-----:|:-----:|
|![team1](https://github.com/UihwanLee/Learning-Enhancement-Platform-Using-ChatPDF/assets/36596037/fc5c2073-fd68-4d21-b52f-83a9fb6dc5f4)|![team2](https://avatars.githubusercontent.com/u/101345563?v=4)|![team3](https://avatars.githubusercontent.com/u/115717251?v=4)|![team4](https://avatars.githubusercontent.com/u/233680955?v=4)|![team5](https://github.com/moon7441-dev)|
|이의환(https://github.com/UihwanLee)|김하늘(https://github.com/Hagill)|전규태(https://github.com/JeonGyuTae)|송정민(https://github.com/song010301-cloud)|김문경(https://github.com/moon7441-dev)|

- 이의환: AI + 전투 시스템
- 김하: 플레이어, 인벤토
- 전규태: 맵&지도, NPC
- 송정민: 건축 시스
- 김문경: 자원 스폰, 수집, 가공

## 3. 기술적 이슈

### 1) Object Culling

<img width="969" height="455" alt="8" src="https://github.com/user-attachments/assets/c6ee19ec-13be-421b-a41c-58cef05342b9" />

- 다수의 AI 오브젝트들이 맵 배치되어 성능 저하 이슈
- 오브젝트 컬링을 Player에 SphereCollider를 부착하여 Trigger 탐지를 통해 구현하려 시도

<pre>
<code>
public interface ICullable 
{
    public void EnableCullComponents();
    public void DisableCullComponents();
}
</code>
</pre>

 - ICuallbe 인터페이스를 구현, 컬링할 오브젝트만 선택하여 오브젝트 컬링할 수 있도록 설계

<pre>
<code>
public virtual void DisableCullComponents()
{
    skinnedMeshRenderer.enabled = false;
    controller.enabled = false;
}
</code>
</pre>

 - Trigger는 비활성화 된 Collider 탐지 불가 -> 오브젝트는 활성화한 채 성능에 영향 주는 요소만 비활성화(Renderer, Script)


