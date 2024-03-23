using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class PacketHandler
{
	public static void S_EnterGameHandler(PacketSession session, IMessage packet)
	{
		S_EnterGame enterGamePacket = packet as S_EnterGame;

        Managers.objectMgr.Add(enterGamePacket.Player, myCtrl: true);

        Debug.Log("S_EnterGameHandler");
        Debug.Log(enterGamePacket.Player);
    }

    public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
    {
        S_LeaveGame leaveGamePacket = packet as S_LeaveGame;
        Managers.objectMgr.RemoveMyPlayer();
    }

    public static void S_SpawnHandler(PacketSession session, IMessage packet)
    {
        S_Spawn spawnPacket = packet as S_Spawn;

        foreach (PlayerInfo player in spawnPacket.PlayerList)
        {
            Managers.objectMgr.Add(player, myCtrl: false);
        }
    }

    public static void S_DespawnHandler(PacketSession session, IMessage packet)
    {
        S_Despawn despawnPacket = packet as S_Despawn;
        foreach (int Id in despawnPacket.PlayerIdList)
        {
            Managers.objectMgr.Remove(Id);
        }
    }

    public static void S_MoveHandler(PacketSession session, IMessage packet)
    {
        S_Move movePacket = packet as S_Move;
        //ServerSession serverSession = session as ServerSession;

        GameObject go = Managers.objectMgr.FindGameObjectbyId(movePacket.PlayerId);
        if (go == null)
            return;

        CreatureCtrl cc = go.GetComponent<CreatureCtrl>();
        if (cc == null)
            return;

        cc.PosInfo = movePacket.PosInfo;
    }

    public static void S_SkillHandler(PacketSession session, IMessage packet)
    {
        S_Skill skillPacket = packet as S_Skill;
        //ServerSession serverSession = session as ServerSession;

        GameObject go = Managers.objectMgr.FindGameObjectbyId(skillPacket.PlayerId);    // 스킬 쓴 플레이어 서치
        if (go == null)
            return;

        PlayerCtrl pc = go.GetComponent<PlayerCtrl>();
        if (pc == null)
            return;

        pc.useSkill(skillPacket.SkillInfo.SkillId);
    }
}
