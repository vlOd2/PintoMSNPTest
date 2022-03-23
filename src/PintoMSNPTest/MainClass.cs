using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using EzLogging;
using PintoMSNPTest.MSNClasses;
using PintoMSNPTest.MSNPClasses;

namespace PintoMSNPTest
{
	/// <summary>
	/// The main class
	/// </summary>
	public class MainClass
	{
		public static MainClass Instance;
		public readonly Logger ServerLogger = new Logger("{h}:{m}:{s} [{head}] {msg}", LogLevel.INFO);
		public IPEndPoint ServerListenEndPoint;
		public Socket TcpServer;
		public Thread AcceptClientThread;
		public bool IsRunning = false;
		public List<MSNPClient> ConnectedClients = new List<MSNPClient>();

		/// <summary>
		/// Starts the MSNP server
		/// </summary>
		public void Start()
		{
			try 
			{
				ServerLogger.Info("Starting MSNP server...");
				ServerListenEndPoint = new IPEndPoint(IPAddress.Any, 1863);
				AcceptClientThread = new Thread(new ThreadStart(AcceptClientThread_Func));
				TcpServer = new Socket(IPAddress.Any.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

				ServerLogger.Info("Binding MSNP server on " + ServerListenEndPoint.ToString());
				TcpServer.Bind(ServerListenEndPoint);

				ServerLogger.Info("Listening on " + ServerListenEndPoint.ToString());
				IsRunning = true;
				TcpServer.Listen(0);
				AcceptClientThread.Start();

				ServerLogger.Info("Server started. Waiting for connections...");
				while (IsRunning) 
				{
					Thread.Sleep(1);
				}
			}
			catch (Exception ex)
			{
				HandleCrash(ex, "FAILED TO START THE SERVER");
				Console.ReadLine();
			}
		}

		/// <summary>
		/// Handles a crash
		/// </summary>
		public void HandleCrash(Exception ex, string msg)
		{
			ServerLogger.Fatal("");
			ServerLogger.Fatal("");
			ServerLogger.Fatal("--------------------------------------------------------------------------------------");
			ServerLogger.Fatal("!!!CRASH!!!");
			ServerLogger.Fatal("");
			ServerLogger.Fatal(msg);
			ServerLogger.Fatal(ex.Message);
			ServerLogger.Fatal("");
			ServerLogger.Fatal("Stacktrace:\n" + ex.Message);
			ServerLogger.Fatal("--------------------------------------------------------------------------------------");
		}

		/// <summary>
		/// Logs a change that occured in a client's status
		/// </summary>
		public void LogClientChange(MSNPClientStateChange newState, MSNPClient client, object additionalData = null)
		{
			switch (newState)
			{
				case MSNPClientStateChange.CLIENT_CONNECT:
					ServerLogger.Info(client.ClientIPAddress + " has connected.");
					break;
				case MSNPClientStateChange.CLIENT_DISCONNECT:
					ServerLogger.Info(client.ClientIPAddress + " has disconnected.");
					break;
				case MSNPClientStateChange.CLIENT_SEND_TCP_DATA:
					ServerLogger.Info(client.ClientIPAddress + " => S -> " + additionalData.ToString());
					break;
				case MSNPClientStateChange.CLIENT_RECEIVE_TCP_DATA:
					ServerLogger.Info("S => " + client.ClientIPAddress + " -> " + additionalData.ToString());
					break;
				default:
					break;
			}
		}

		/// <summary>
		/// Gets an account using the specified e-mail address
		/// </summary>
		public MSNAccount GetAccountByEmail(string email)
		{
			foreach (MSNPClient client in ConnectedClients)
			{
				if (email.Equals(client.AssociatedAccount.EmailAddress))
				{
					return client.AssociatedAccount;
				}
			}

			return null;
		}

		/// <summary>
		/// The function for the accept thread
		/// </summary>
		private void AcceptClientThread_Func()
		{
			while (IsRunning)
			{
				Socket newTcpClient = TcpServer.Accept();
				bool gotKicked = false;

				foreach (MSNPClient connectedClient in ConnectedClients.ToArray())
				{
					if (connectedClient.TcpClientHandler.TcpClientIPEndPoint.Address.ToString().Equals(((IPEndPoint)newTcpClient.RemoteEndPoint).Address.ToString()))
					{
						newTcpClient.Close();
						gotKicked = true;
						break;
					}
				}

				if (gotKicked)
					continue;

				MSNPClient newClient = new MSNPClientImpl(newTcpClient);
				ConnectedClients.Add(newClient);

				LogClientChange(MSNPClientStateChange.CLIENT_CONNECT, newClient);
			}
		}
	}
}