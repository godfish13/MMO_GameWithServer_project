using System;
using System.Collections.Generic;
using System.Text;

namespace Server.InGame
{
    public class PlayerMgr
    {
        public static PlayerMgr Instance { get; } = new PlayerMgr();

        object _lock = new object();
        Dictionary<int, Player> _players = new Dictionary<int, Player>();
        int _playerId = 1;  // TODO
        // 플레이어 아이디 하나 딸랑 담기에 int는 너무 큼 그러므로 수정예정

        public Player Add()
        {
            Player player = new Player();

            lock (_lock)
            {
                player.info.PlayerId = _playerId;   // 플레이어 추가시 playerId 발급
                _players.Add(_playerId, player);    // 해당 아이디와 플레이어 list에 기록
                _playerId++;
            }
            return player;
        }

        public bool Remove(int playerId)
        {
            lock (_lock)
            {
                return _players.Remove(playerId);
            }
        }

        public Player Find(int playerId)
        {
            lock (_lock)
            {
                Player player = null;
                if (_players.TryGetValue(playerId, out player))
                    return player;

                return null;
            }
        }
    }
}
