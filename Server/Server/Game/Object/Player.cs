using Google.Protobuf.Protocol;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class Player : GameObject
    {       
        public ClientSession mySession { get; set; }
        public int Level { get; set; } = 1;

        public Player()
        {
            ObjectType = GameObjectType.Player;
            Data.Stat statData = null;
            if (DataMgr.StatDictionary.TryGetValue(key: 1, out statData) == true)    // 플레이어 레벨을 key값으로 Json의 speed 지정
            {                                                                        // 현재 Player의 레벨 구현 안해뒀으므로 1로 하드코딩
                Speed = statData.speed;
            }
            else
            {
                Speed = 1.0f;   // 잘못된 레벨이 들어갈 경우 1.0f로 밀어버림
            }
        }

        public override void OnDamaged(GameObject attacker, int damage)
        {
            Console.WriteLine($"Todo : damage : {damage}");
            Console.WriteLine($"damaged obejct speed : {Speed}");
        }
    }
}
