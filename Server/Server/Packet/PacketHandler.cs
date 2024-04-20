﻿using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;
using Server.Game;

class PacketHandler
{
	public static void C_MoveHandler(PacketSession session, IMessage packet)
	{
        C_Move movePacket = packet as C_Move;
		ClientSession clientSession = session as ClientSession;

		//Console.WriteLine($"C_Move ({movePacket.PosInfo.PosX}, {movePacket.PosInfo.PosY})");

		Player player = clientSession.myPlayer;		// clientSession.myPlayer == null 체크는 멀티스레드 환경 상 제대로 작동안할 가능성이 존재함
		if (player == null)				// 이를 방지하기 위해 player로 한번 꺼내고 null체크하면 다른 스레드에서 myPlayer을 건들더라도  
			return;                     // player는 그대로 남아있으므로 비교적 안전해짐

        GameRoom Room = player.MyRoom;	// player랑 마찬가지
		if (Room == null)				// 얘는 GameRoom.LeaveGame에서 실제로 null로 밀어주기도 함으로 더위험함!! 더 주의해야하는부분
			return;

		//Todo 검증(클라이언트가 잘못된 정보 보내는지?)

		Room.Push(Room.HandleMove, player, movePacket);
	}

	public static void C_SkillHandler(PacketSession session, IMessage packet)
	{
        C_Skill skillPacket = packet as C_Skill;
        ClientSession clientSession = session as ClientSession;

        Player player = clientSession.myPlayer;     // clientSession.myPlayer == null 체크는 멀티스레드 환경 상 제대로 작동안할 가능성이 존재함
        if (player == null)             // 이를 방지하기 위해 player로 한번 꺼내고 null체크하면 다른 스레드에서 myPlayer을 건들더라도  
            return;                     // player는 그대로 남아있으므로 비교적 안전해짐

        GameRoom Room = player.MyRoom;  // player랑 마찬가지
        if (Room == null)               // 얘는 GameRoom.LeaveGame에서 실제로 null로 밀어주기도 함으로 더위험함!! 더 주의해야하는부분
            return;

		Room.Push(Room.HandleSkill, player, skillPacket);
    }
}
