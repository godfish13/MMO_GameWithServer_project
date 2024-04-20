using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;
using System.Net;
using Google.Protobuf.Protocol;
using Google.Protobuf;
using System.Diagnostics;
using Server.Game;
using Server.Data;

namespace Server
{
    public class ClientSession : PacketSession
	{
		public Player myPlayer { get; set; }	// 이 Session을 가진 Player
		public int SessionId { get; set; }

		public void Send(IMessage packet)
		{
			string MsgName = packet.Descriptor.Name.Replace("_", string.Empty); // Descriptor.Name : 패킷의 이름 꺼내옴 / "_"는 실제 실행시 무시되기때문에 없애줌
			MsgId msgId = (MsgId)Enum.Parse(typeof(MsgId), MsgName);	// Enum.Parse(Type, string) : string과 같은 이름을 지닌 Type을 뱉어줌

            ushort size = (ushort)packet.CalculateSize();
            byte[] sendBuffer = new byte[size + 4];	// 제일 앞에 패킷크기, 다음에 패킷 Id 넣어줄 공간 4byte(ushort 2개) 추가
            Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));	// 패킷 크기 // GetBytes(ushort)로 쬐꼼이라도 성능향상...
            Array.Copy(BitConverter.GetBytes((ushort)msgId), 0, sendBuffer, 2, sizeof(ushort));	// 패킷 Id
            Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);						// 패킷 내용

            Send(new ArraySegment<byte>(sendBuffer));		
        }

		public override void OnConnected(EndPoint endPoint)
		{
			Console.WriteLine($"OnConnected : {endPoint}");

			myPlayer = ObjectMgr.Instance.Add<Player>();    // 플레이어 목록에 접속한 플레이어 넣고 자신이 대변하는 플레이어 기록

            #region 플레이어 정보 입력
            {
                myPlayer.Info.Name = $"Player_{myPlayer.Info.ObjectId}";
                myPlayer.Info.PosInfo.State = CreatureState.Idle;
				myPlayer.Info.PosInfo.MoveDir = MoveDir.Down;
				myPlayer.Info.PosInfo.PosX = 0;
				myPlayer.Info.PosInfo.PosY = 0;

				StatInfo _stat = null;									// StatJson으로 불러놓은 player정보 넣기
				DataMgr.StatDictionary.TryGetValue(key: 1, out _stat);	// 처음 접속하는것이므로 레벨1로 하드코딩
				myPlayer.Stat.MergeFrom(_stat);

                myPlayer.mySession = this;
            }
			#endregion

			GameRoom gameRoom = RoomMgr.Instance.Find(1);
			gameRoom.Push(gameRoom.EnterGame, myPlayer);
            Console.WriteLine($"{myPlayer.Info.Name} has entered to GameRoom_{RoomMgr.Instance.Find(1).RoomId}");
        }

		public override void OnRecvPacket(ArraySegment<byte> buffer)
		{
			PacketManager.Instance.OnRecvPacket(this, buffer);
			//Console.WriteLine(buffer);
		}

		public override void OnDisconnected(EndPoint endPoint)
		{
			SessionManager.Instance.Remove(this);

			GameRoom gameRoom = RoomMgr.Instance.Find(1);
			gameRoom.Push(gameRoom.LeaveGame, myPlayer.Info.ObjectId);
			
            Console.WriteLine($"OnDisconnected : {endPoint}");
		}

		public override void OnSend(int numOfBytes)
		{
			//Console.WriteLine($"Transferred bytes: {numOfBytes}");
		}
	}
}
