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

		static void FlushRoom()
		{
			JobTimer.Instance.Push(FlushRoom, 250);
		}

		static void Main(string[] args)
		{
			ConfigMgr.LoadConfig();	// Config 파일 읽기
			DataMgr.LoadData();     // 데이터 읽어들여오기

			RoomMgr.Instance.Add(1);
			Console.WriteLine($"Current Room : {RoomMgr.Instance.Find(1).RoomId}");

			// DNS (Domain Name System)
			string host = Dns.GetHostName();
			IPHostEntry ipHost = Dns.GetHostEntry(host);
			IPAddress ipAddr = ipHost.AddressList[0];
			IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

			_listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
			Console.WriteLine("Listening...");

			// Todo JobTimer
			while (true)
			{
				GameRoom gameRoom = RoomMgr.Instance.Find(1);
				gameRoom.Push(gameRoom.Update);
				Thread.Sleep(10);
			}
		}
	}
}
