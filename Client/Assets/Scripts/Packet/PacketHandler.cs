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
        Managers.objectMgr.Clear();
    }

    public static void S_SpawnHandler(PacketSession session, IMessage packet)
    {
        S_Spawn spawnPacket = packet as S_Spawn;

        foreach (ObjectInfo gameObject in spawnPacket.ObjectList)
        {
            Managers.objectMgr.Add(gameObject, myCtrl: false);
        }
    }

    public static void S_DespawnHandler(PacketSession session, IMessage packet)
    {
        S_Despawn despawnPacket = packet as S_Despawn;
        foreach (int Id in despawnPacket.ObjectIdList)
        {
            Managers.objectMgr.Remove(Id);
        }
    }

    public static void S_MoveHandler(PacketSession session, IMessage packet)
    {
        S_Move movePacket = packet as S_Move;

        GameObject go = Managers.objectMgr.FindGameObjectbyId(movePacket.ObjectId);
        if (go == null)
            return;

        BaseCtrl bc = go.GetComponent<BaseCtrl>();
        if (bc == null)
            return;

        bc.PosInfo = movePacket.PosInfo;
    }

    public static void S_SkillHandler(PacketSession session, IMessage packet)
    {
        S_Skill skillPacket = packet as S_Skill;
        //ServerSession serverSession = session as ServerSession;

        //Debug.Log($"{skillPacket.ObjectId} used Skill");
        GameObject go = Managers.objectMgr.FindGameObjectbyId(skillPacket.ObjectId);    // 스킬 쓴 오브젝트 서치
        if (go == null)
        {
            Debug.Log("null go");
            return;
        }
            
        CreatureCtrl cc = go.GetComponent<CreatureCtrl>();
        if (cc == null)
        {
            Debug.Log("null cc");
            return;
        }
            
        cc.UseSkill(skillPacket.SkillInfo.SkillId);
    }

    public static void S_ChangeHpHandler(PacketSession session, IMessage packet)
    {
        S_ChangeHp changeHpPacket = packet as S_ChangeHp;
        //ServerSession serverSession = session as ServerSession;

        GameObject go = Managers.objectMgr.FindGameObjectbyId(changeHpPacket.ObjectId);    // 맞은 오브젝트 서치
        if (go == null)
            return;

        CreatureCtrl cc = go.GetComponent<CreatureCtrl>();
        if (cc == null)
            return;

        cc.Hp = changeHpPacket.Hp;
        Debug.Log($"{ObjectMgr.GetDecimalId(cc.Id)} player get {changeHpPacket.DeltaHp} damage");
        Debug.Log($"{ObjectMgr.GetDecimalId(cc.Id)} player Hp : {cc.Stat.Hp}"); 
    }

    public static void S_OnDeadHandler(PacketSession session, IMessage packet)
    {
        S_OnDead diePacket = packet as S_OnDead;
        //ServerSession serverSession = session as ServerSession;

        GameObject go = Managers.objectMgr.FindGameObjectbyId(diePacket.ObjectId);    // 맞은 오브젝트 서치
        if (go == null)
            return;

        CreatureCtrl cc = go.GetComponent<CreatureCtrl>();
        if (cc == null)
            return;

        cc.Hp = 0;  // 혹시 죽었는데 변수에의해 Hp != 0인 버그 제거
        cc.OnDead();

    }
}
