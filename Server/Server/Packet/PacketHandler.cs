using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

class PacketHandler
{
	public static void C_MoveHandler(PacketSession session, IMessage packet)
	{
        C_Move movePacket = packet as C_Move;
		ClientSession clientSession = session as ClientSession;

		Console.WriteLine($"C_Move ({movePacket.PosInfo.PosX}, {movePacket.PosInfo.PosY})");
		Console.WriteLine($"LastDir : {movePacket.PosInfo.LastDir}");

		if (clientSession.myPlayer == null)
			return;
		if (clientSession.myPlayer.myRoom == null)
			return;

		//Todo 검증(클라이언트가 잘못된 정보 보내는지?)

		// 서버에 저장된 자신의 좌표 변경(이동)
		PlayerInfo info = clientSession.myPlayer.info;
		info.PosInfo = movePacket.PosInfo;

		// 다른 플레이어들에게 자기위치 방송
		S_Move broadMovePkt = new S_Move();	// 방송하려고 서버측에서 보내는 M 패킷
		broadMovePkt.PlayerId = clientSession.myPlayer.info.PlayerId;	// 움직인 자신 Id 입력
		broadMovePkt.PosInfo = movePacket.PosInfo;

		clientSession.myPlayer.myRoom.BroadCast(broadMovePkt);
	}
}
