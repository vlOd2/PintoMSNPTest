using System;
using PintoMSNPTest.MSNClasses;
using System.Linq;
using System.Net.Sockets;

namespace PintoMSNPTest.MSNPClasses
{
	/// <summary>
	/// An implentation of the MSNP client
	/// </summary>
	public class MSNPClientImpl : MSNPClient
	{
		private string loginSaltStr = "";

		/// <summary>
		/// An implentation of the MSNP client
		/// </summary>
		public MSNPClientImpl(Socket tcpClient) : base(tcpClient) {}

		/// <summary>
		/// Handles a received packet
		/// </summary>
		public override void HandlePacket(MSNPPacket packet)
		{
			try 
			{
				mainInstance.LogClientChange(MSNPClientStateChange.CLIENT_SEND_TCP_DATA, this, packet.ToString());
				MSNPPacket toSendPacket = MSNPPacket.CreateEmptyPacket();
				toSendPacket.PacketTransactionID = packet.PacketTransactionID;

				if (!IsLoggedIn && !ALLOWED_LOGOUT_COMMANDS.Contains(packet.PacketCommand))
				{
					Disconnect();
					return;
				}

				switch (packet.PacketCommand)
				{
					case "VER":
						if (isLoggedIn)
						{
							SendErrorCode(MSNPErrorCode.CODE_207, packet.PacketTransactionID);
							return;
						}

						int clientLatestProtocolVersion = Int32.Parse(((string)packet.GetData(0)).Trim().Replace("MSNP", string.Empty));

						if (clientLatestProtocolVersion > 13)
							clientLatestProtocolVersion = 13;

						protocolVersion = clientLatestProtocolVersion;

						toSendPacket.PacketCommand = "VER";
						toSendPacket.AddData("MSNP" + protocolVersion);
						toSendPacket.AddData("CVR0");

						break;
					case "INF":
						if (isLoggedIn)
						{
							SendErrorCode(MSNPErrorCode.CODE_207, packet.PacketTransactionID);
							return;
						}

						toSendPacket.PacketCommand = "INF";
						toSendPacket.AddData("MD5");

						break;
					case "USR":
						if (isLoggedIn)
						{
							SendErrorCode(MSNPErrorCode.CODE_207, packet.PacketTransactionID);
							return;
						}

						string typeOfUSR = (string)packet.GetData(1);
						string usrValue = (string)packet.GetData(2);
						string usrValue2 = (string)packet.GetData(3);

						if (typeOfUSR.Equals("I"))
						{
							// Failed
							if (!HandleUSR(false, usrValue))
							{
								SendErrorCode(MSNPErrorCode.CODE_911, packet.PacketTransactionID);
								Disconnect();
								return;
							}
							else 
							{
								toSendPacket.PacketCommand = "USR";
								toSendPacket.AddData("MD5");
								toSendPacket.AddData("S");
								toSendPacket.AddData(loginSaltStr);
							}
						}
						else if (typeOfUSR.Equals("S"))
						{
							// Failed
							if (!HandleUSR(true, usrValue))
							{
								SendErrorCode(MSNPErrorCode.CODE_911, packet.PacketTransactionID);
								Disconnect();
								return;
							}
							else 
							{
								toSendPacket.PacketCommand = "USR";
								toSendPacket.AddData("OK");
								toSendPacket.AddData(AssociatedAccount.EmailAddress);
								toSendPacket.AddData(AssociatedAccount.DisplayName);
							}
						}
						else
						{
							Disconnect();
							return;
						}

						break;
					case "SYN":
						toSendPacket.PacketCommand = "SYN";
						toSendPacket.AddData(LIST_VERSION);

						TcpClientHandler.SendData(toSendPacket.ToString() + Environment.NewLine);
						ClientSync.SyncDataToClient(packet.PacketTransactionID);

						return;
					case "CHG":
						string newStatus = (string)packet.GetData(0);
						MSNPStatus parsedNewStatus = Tools.StatusFromStr(newStatus);

						AssociatedAccount.Status = parsedNewStatus;
						toSendPacket.PacketCommand = "CHG";
						toSendPacket.AddData(newStatus);

						break;
					case "CVR":
						toSendPacket.PacketCommand = "CVR";
						toSendPacket.AddData("4.7.0105");
						toSendPacket.AddData("4.7.0105");
						toSendPacket.AddData("1.0.0000");
						toSendPacket.AddData("http://ftp.xomi.xyz/Media/Magar.mp4");
						toSendPacket.AddData("http://ftp.xomi.xyz/Media/Magar.mp4");
						break;
					case "OUT":
						toSendPacket.PacketCommand = "OUT";
						SendPacket(toSendPacket);
						Disconnect();
						return;
					case "ADD":
						/*
						string targetedList = (string)packet.GetData(0);
						string targetedAccount = (string)packet.GetData(1);
						MSNAccount foundAccount = mainInstance.GetAccountByEmail(targetedAccount);

						if (foundAccount == null)
						{
							if (!(protocolVersion < 4))
								SendErrorCode(MSNPErrorCode.CODE_208, packet.PacketTransactionID);
							else
								SendErrorCode(MSNPErrorCode.CODE_205, packet.PacketTransactionID);
							return;
						}
						else 
						{
							if (targetedList.Equals("BL"))
							{
								if (!(protocolVersion < 4))
									SendErrorCode(MSNPErrorCode.CODE_208, packet.PacketTransactionID);
								else
									SendErrorCode(MSNPErrorCode.CODE_205, packet.PacketTransactionID);
								return;
							}

							toSendPacket.PacketCommand = "ADD";
							toSendPacket.AddData(targetedList);
							toSendPacket.AddData(LIST_VERSION);
							toSendPacket.AddData(targetedAccount);
							toSendPacket.AddData(targetedAccount);
							AssociatedAccount.AddContact(foundAccount.ContactEntry);
						}*/

						if (!(protocolVersion < 4))
							SendErrorCode(MSNPErrorCode.CODE_208, packet.PacketTransactionID);
						else
							SendErrorCode(MSNPErrorCode.CODE_205, packet.PacketTransactionID);
						return;
				}	

				SendPacket(toSendPacket);
			}
			catch (Exception ex)
			{
				mainInstance.ServerLogger.Error(ex.ToString());
				Disconnect();
				return;
			}
		}

		/// <summary>
		/// Handles a received USR command
		/// </summary>
		private bool HandleUSR(bool isSecond, string usrValue)
		{
			if (!isSecond)
			{
				AssociatedAccount = new MSNAccount(this, usrValue);
				ClientSync = new MSNPClientSync(this);
				loginSaltStr = "BOOST";
				return true;
			}
			else
			{
				isLoggedIn = true;

				foreach (MSNPClient connectedClient in mainInstance.ConnectedClients.ToArray())
				{
					MSNAccount clientAccount = connectedClient.AssociatedAccount;

					foreach (MSNContact clientAccountContact in clientAccount.AllowedList.ToArray())
					{
						if (clientAccountContact.EmailAddress.Equals(AssociatedAccount.EmailAddress))
						{
							clientAccount.AddContact(AssociatedAccount.ContactEntry);
						}	
					}
				}

				return true;
			}
		}
	}
}