using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using Google.Protobuf.WellKnownTypes;
using ServerCore;
using Server.Game;
using Server.Data;

namespace Server
{
    class Program
	{
		static Listener _listener = new Listener();
		static List<System.Timers.Timer> _timers = new List<System.Timers.Timer>();	// Timer들 관리하기위해 들고있기

		static void TickRoom(GameRoom room, int tick = 100)
		{
			var timer = new System.Timers.Timer();
			timer.Interval = tick;  // 실행 간격
			timer.Elapsed += (s, e) => { room.Update(); };  // 실행 간격마다 실행시키고 싶은 이벤트 등록
                             // s, e는 각각 sender(이벤트 발생시킨 객체, 보통 Timer자신)
                             // e는 이벤트와 관련된 정보가 담겨짐, 예를들어 e.SignalTime으로 타이머 만료된 시간 정보 획득 가능 (GPT 답변, word에 옮겨둠 참고)
            timer.AutoReset = true;	// 리셋해주기
			timer.Enabled = true;	// 타이머 실행

			_timers.Add(timer);
			//timer.Stop();	// 타이머 정지
		}

		static void Main(string[] args)
		{
			ConfigMgr.LoadConfig();	// Config 파일 읽기
			DataMgr.LoadData();     // 데이터 읽어들여오기

			GameRoom currentRoom = RoomMgr.Instance.Add(1);
			TickRoom(currentRoom, 10);	// 50ms 딜레이 주면서 currentRoom.Update 실행
			Console.WriteLine($"Current Room : {RoomMgr.Instance.Find(1).RoomId}");

			// DNS (Domain Name System)
			string host = Dns.GetHostName();
			IPHostEntry ipHost = Dns.GetHostEntry(host);
			IPAddress ipAddr = ipHost.AddressList[0];
			IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

			_listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
			Console.WriteLine("Listening...");

			// 프로그램 안꺼지게 유지
			while (true)
			{
				Thread.Sleep(10);
			}
		}
	}
}
