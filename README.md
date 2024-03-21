# MMO_GameWithServer_project
 portfolio project focused on connecting Game to Server for Multiplayer online game

PlayerInfo 내부에 MoveDir lastDir 을 추가하고 CreatureCtrl의 LastDir을 C_Move 패킷에 같이 포함시켜 보내고 그에따라 서버의 playerMgr에 저장해둠  

이에따라 새로운 플레이어가 추가되고 이미 접속해있던 다른 플레이어들의 정보를 받을 시, lastDir의 정보도 같이 받게하여 다른 플레이어들이 생성될 때 그들이 바라보던 방향을 바라본채로 생성하는 것에 성공함

그러나, 사실상 논 플레이어블 플레이어 캐릭터 생성때 단 한번 쓰기 위한 내용을 굳이 패킷에 포함시키는건 패킷 용량 낭비 + 서버 통신 시간 낭비 아닌가? 라는 의문점이 남음  

플레이어가 접속한동안 다른 플레이어 객체들이 한번이라도 움직이면 lastDir또한 알아서 변경되고 그에따른 방향을 유지하기 떄문

그러므로 이 파트는 일단 제거 후 진행하고 sub브렌치에 저장해둠
