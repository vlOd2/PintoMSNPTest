using System;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace PintoMSNPTest.MSNPClasses
{
	/// <summary>
	/// A class that represents a TCP handler for an MSNP client
	/// </summary>
	public class MSNPClientTCPHandler
	{
		private MainClass mainInstance;
		private MSNPClient associatedClient;
		private bool isConnected;
		private Socket tcpServer;
		private Socket tcpClient;
		private IPEndPoint tcpClientIPEndPoint;
		private Thread receiveDataThread;

		/// <summary>
		/// The client's IPEndPoint
		/// </summary>
		public IPEndPoint TcpClientIPEndPoint { get { return tcpClientIPEndPoint; } }
		/// <summary>
		/// Gets a value indicating whether this client is connected
		/// </summary>
		public bool IsConnected { get { return isConnected; } }

		/// <summary>
		/// A class that represents a TCP handler for an MSNP client
		/// </summary>
		public MSNPClientTCPHandler(MSNPClient associatedClient, Socket tcpClient)
		{
			mainInstance = MainClass.Instance;
			isConnected = true;
			this.associatedClient = associatedClient;

			this.tcpClient = tcpClient;
			tcpClientIPEndPoint = (IPEndPoint)this.tcpClient.RemoteEndPoint;
			tcpServer = MainClass.Instance.TcpServer;
			receiveDataThread = new Thread(new ThreadStart(ReceiveDataThread_Func));

			receiveDataThread.Start();
		}

		/// <summary>
		/// Sends data to the client
		/// </summary>
		public void SendData(string data)
		{
			if (!isConnected)
				return;
			
			byte[] encodedData = Encoding.UTF8.GetBytes(data);
			tcpClient.Send(encodedData, 0, encodedData.Length, SocketFlags.None);
			mainInstance.LogClientChange(MSNPClientStateChange.CLIENT_RECEIVE_TCP_DATA, associatedClient, data.Trim());
		}

		/// <summary>
		/// Disconnects the client
		/// </summary>
		public void Disconnect()
		{
			if (!isConnected)
				return;

			isConnected = false;
			mainInstance.LogClientChange(MSNPClientStateChange.CLIENT_DISCONNECT, associatedClient);
			associatedClient.Disconnect();

			tcpClient.Close();
			tcpClient.Dispose();
			tcpClientIPEndPoint = null;
			receiveDataThread = null;
		}

		/// <summary>
		/// Function called when data is received
		/// </summary>
		private void OnReceivedData(byte[] data)
		{
			MSNPPacket packet = MSNPPacket.CreatePacketFromParsedData(Encoding.UTF8.GetString(data).Trim());
			associatedClient.HandlePacket(packet);
		}

		/// <summary>
		/// Gets a value indicating whether the specified byte is an end byte
		/// </summary>
		private bool IsEndByte(byte b)
		{
			return (b == 0x0A || b == 0x0D);
		} 

		/// <summary>
		/// Gets a value indicating whether the specified final data is valid
		/// </summary>
		private bool IsFinalDataValid(byte[] finalData)
		{
			return !(finalData.Length <= 1 ||
				IsEndByte(finalData[0]) ||
				finalData[0] == 0x20);
		}

		/// <summary>
		/// The function for the receive data thread
		/// </summary>
		private void ReceiveDataThread_Func()
		{
			while (isConnected)
			{
				List<byte> receivedData = new List<byte>();
				byte[] receivedByte = new byte[1] { 0x01 };

				while (!IsEndByte(receivedByte[0]))
				{
					try 
					{
						// Check if data available
						if (tcpClient.Poll(0, SelectMode.SelectRead))
						{
							// First method of checking if disconnected
							if (tcpClient.Receive(receivedByte, 0, 1, SocketFlags.None) == 0)
								throw new SocketException();
							else 
								// Second method of checking if disconnected
								if (receivedByte[0] == 0x00)
									throw new SocketException();
								else if (!IsEndByte(receivedByte[0]))
									receivedData.Add(receivedByte[0]);
						}
					}
					catch (SocketException)
					{
						Disconnect();
						return;
					}

					Thread.Sleep(1);
				}

				byte[] finalDataArray = receivedData.ToArray();
				if (IsFinalDataValid(finalDataArray))
					OnReceivedData(finalDataArray);
			}	
		}
	}
}