using System;
using System.Text;
using System.Linq;
using PintoMSNPTest.MSNClasses;
using System.Net.Sockets;

namespace PintoMSNPTest.MSNPClasses
{
	/// <summary>
	/// A class that represents an MSNP client
	/// </summary>
	public abstract class MSNPClient
	{
		public static readonly string LIST_VERSION = "12";
		public static readonly string[] ALLOWED_LOGOUT_COMMANDS = new string[] { "VER", "INF", "USR", "CVR" };

		protected MainClass mainInstance = MainClass.Instance;
		protected int protocolVersion = 0;
		protected bool isLoggedIn = false;

		/// <summary>
		/// The client's sync manager
		/// </summary>
		public MSNPClientSync ClientSync = null;
		/// <summary>
		/// The client's TCP handler
		/// </summary>
		public MSNPClientTCPHandler TcpClientHandler = null;
		/// <summary>
		/// The client's events manager
		/// </summary>
		public readonly MSNPClientEventsManager ClientEventsManager = null;
		/// <summary>
		/// The associated MSN account
		/// </summary>
		public MSNAccount AssociatedAccount = null;
		/// <summary>
		/// Value indicating the client's IP address
		/// </summary>
		public string ClientIPAddress { get { return ((TcpClientHandler.TcpClientIPEndPoint != null) ? TcpClientHandler.TcpClientIPEndPoint.ToString() : ""); } }
		/// <summary>
		/// Value indicating the protocol version used
		/// </summary>
		public int ProtocolVersion { get { return protocolVersion; } }
		/// <summary>
		/// Value indicating whether the client is logged in or not
		/// </summary>
		public bool IsLoggedIn { get { return isLoggedIn; } }

		/// <summary>
		/// A class that represents an MSNP client
		/// </summary>
		public MSNPClient(Socket tcpClient)
		{
			TcpClientHandler = new MSNPClientTCPHandler(this, tcpClient);
			ClientEventsManager = new MSNPClientEventsManager(this);
		}

		/// <summary>
		/// Handles a received packet
		/// </summary>
		public abstract void HandlePacket(MSNPPacket packet);

		/// <summary>
		/// Sends a packet to the client
		/// </summary>
		public void SendPacket(MSNPPacket toSendPacket)
		{
			TcpClientHandler.SendData(toSendPacket.ToString() + Environment.NewLine);
		}

		/// <summary>
		/// Sends an error code to the specified transaction
		/// </summary>
		public void SendErrorCode(MSNPErrorCode errCode, int transactionID)
		{
			MSNPPacket toSendPacket = MSNPPacket.CreateEmptyPacket();
			toSendPacket.PacketCommand = Tools.ErrCodeToStr(errCode);
			toSendPacket.PacketTransactionID = transactionID;
			SendPacket(toSendPacket);
		}

		/// <summary>
		/// Disconnects the client
		/// </summary>
		public void Disconnect()
		{
			if (AssociatedAccount != null)
				AssociatedAccount.Status = MSNPStatus.OFFLINE;

			if (TcpClientHandler.IsConnected)
				TcpClientHandler.Disconnect();

			mainInstance.ConnectedClients.Remove(this);
			isLoggedIn = false;
		}
	}
}