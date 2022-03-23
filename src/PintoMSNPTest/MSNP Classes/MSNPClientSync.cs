using System;
using PintoMSNPTest.MSNClasses;
using System.Linq;

namespace PintoMSNPTest.MSNPClasses
{
	/// <summary>
	/// A class that represents a synchronizer for syncing a client
	/// </summary>
	public class MSNPClientSync
	{
		private MSNPClient associatedClient;
		private MSNAccount associatedAccount;
		private bool isDoingSync = false;

		/// <summary>
		/// Gets a value indicating whether this instance is doing a sync
		/// </summary>
		public bool IsDoingSync { get { return isDoingSync; } }

		/// <summary>
		/// A class that represents a synchronizer for syncing a client
		/// </summary>
		public MSNPClientSync(MSNPClient associatedClient)
		{
			this.associatedClient = associatedClient;
			associatedAccount = this.associatedClient.AssociatedAccount;
		}

		/// <summary>
		/// Syncs and sends the client's data to the client
		/// </summary>
		public void SyncDataToClient(int transactionID)
		{
			isDoingSync = true;
			MSNPPacket toSendPacket = null;

			toSendPacket = MSNPPacket.CreateEmptyPacket();
			toSendPacket.PacketTransactionID = transactionID;
			toSendPacket.PacketCommand = "GTC";
			toSendPacket.AddData(MSNPClient.LIST_VERSION);
			toSendPacket.AddData((associatedClient.AssociatedAccount.ContactPrivacy == true ? "N" : "A"));
			associatedClient.TcpClientHandler.SendData(toSendPacket.ToString() + Environment.NewLine);

			toSendPacket = MSNPPacket.CreateEmptyPacket();
			toSendPacket.PacketTransactionID = transactionID;
			toSendPacket.PacketCommand = "BLP";
			toSendPacket.AddData(MSNPClient.LIST_VERSION);
			toSendPacket.AddData((associatedClient.AssociatedAccount.MsgPrivacy == true ? "BL" : "AL"));
			associatedClient.TcpClientHandler.SendData(toSendPacket.ToString() + Environment.NewLine);

			SyncLists(transactionID);
			UpdateContactsStatuses();
			isDoingSync = false;
		}

		/// <summary>
		/// Syncs the client's lists
		/// </summary>
		public void SyncLists(int transactionID = 0)
		{
			MSNPPacket toSendPacket = null;

			if (associatedAccount.ForwardList.Count < 1)
			{
				toSendPacket = MSNPPacket.CreateEmptyPacket();
				toSendPacket.PacketTransactionID = transactionID;
				toSendPacket.PacketCommand = "LST";
				toSendPacket.AddData("FL");
				toSendPacket.AddData(MSNPClient.LIST_VERSION);
				toSendPacket.AddData("0");
				toSendPacket.AddData("0");
				associatedClient.TcpClientHandler.SendData(toSendPacket.ToString() + Environment.NewLine);
			}
			else
			{
				for (int i = 0; i < associatedAccount.ForwardList.Count; i++)
				{
					toSendPacket = MSNPPacket.CreateEmptyPacket();
					toSendPacket.PacketTransactionID = transactionID;
					toSendPacket.PacketCommand = "LST";
					toSendPacket.AddData("FL");
					toSendPacket.AddData(MSNPClient.LIST_VERSION);
					toSendPacket.AddData(i + 1);
					toSendPacket.AddData(associatedAccount.ForwardList.Count);
					toSendPacket.AddData(associatedAccount.ForwardList[i].EmailAddress);
					toSendPacket.AddData(associatedAccount.ForwardList[i].DisplayName);
					associatedClient.TcpClientHandler.SendData(toSendPacket.ToString() + Environment.NewLine);
				}
			}

			if (associatedAccount.AllowedList.Count < 1)
			{
				toSendPacket = MSNPPacket.CreateEmptyPacket();
				toSendPacket.PacketTransactionID = transactionID;
				toSendPacket.PacketCommand = "LST";
				toSendPacket.AddData("AL");
				toSendPacket.AddData(MSNPClient.LIST_VERSION);
				toSendPacket.AddData("0");
				toSendPacket.AddData("0");
				associatedClient.TcpClientHandler.SendData(toSendPacket.ToString() + Environment.NewLine);
			}
			else
			{
				for (int i = 0; i < associatedAccount.AllowedList.Count; i++)
				{
					toSendPacket = MSNPPacket.CreateEmptyPacket();
					toSendPacket.PacketTransactionID = transactionID;
					toSendPacket.PacketCommand = "LST";
					toSendPacket.AddData("AL");
					toSendPacket.AddData(MSNPClient.LIST_VERSION);
					toSendPacket.AddData(i + 1);
					toSendPacket.AddData(associatedAccount.AllowedList.Count);
					toSendPacket.AddData(associatedAccount.AllowedList[i].EmailAddress);
					toSendPacket.AddData(associatedAccount.AllowedList[i].DisplayName);
					associatedClient.TcpClientHandler.SendData(toSendPacket.ToString() + Environment.NewLine);
				}
			}

			if (associatedAccount.BlockedList.Count < 1)
			{
				toSendPacket = MSNPPacket.CreateEmptyPacket();
				toSendPacket.PacketTransactionID = transactionID;
				toSendPacket.PacketCommand = "LST";
				toSendPacket.AddData("BL");
				toSendPacket.AddData(MSNPClient.LIST_VERSION);
				toSendPacket.AddData("0");
				toSendPacket.AddData("0");
				associatedClient.TcpClientHandler.SendData(toSendPacket.ToString() + Environment.NewLine);
			}
			else
			{
				for (int i = 0; i < associatedAccount.BlockedList.Count; i++)
				{
					toSendPacket = MSNPPacket.CreateEmptyPacket();
					toSendPacket.PacketTransactionID = transactionID;
					toSendPacket.PacketCommand = "LST";
					toSendPacket.AddData("BL");
					toSendPacket.AddData(MSNPClient.LIST_VERSION);
					toSendPacket.AddData(i + 1);
					toSendPacket.AddData(associatedAccount.BlockedList.Count);
					toSendPacket.AddData(associatedAccount.BlockedList[i].EmailAddress);
					toSendPacket.AddData(associatedAccount.BlockedList[i].DisplayName);
					associatedClient.TcpClientHandler.SendData(toSendPacket.ToString() + Environment.NewLine);
				}
			}

			if (associatedAccount.ReverseList.Count < 1)
			{
				toSendPacket = MSNPPacket.CreateEmptyPacket();
				toSendPacket.PacketTransactionID = transactionID;
				toSendPacket.PacketCommand = "LST";
				toSendPacket.AddData("RL");
				toSendPacket.AddData(MSNPClient.LIST_VERSION);
				toSendPacket.AddData("0");
				toSendPacket.AddData("0");
				associatedClient.TcpClientHandler.SendData(toSendPacket.ToString() + Environment.NewLine);
			}
			else
			{
				for (int i = 0; i < associatedAccount.ReverseList.Count; i++)
				{
					toSendPacket = MSNPPacket.CreateEmptyPacket();
					toSendPacket.PacketTransactionID = transactionID;
					toSendPacket.PacketCommand = "LST";
					toSendPacket.AddData("RL");
					toSendPacket.AddData(MSNPClient.LIST_VERSION);
					toSendPacket.AddData(i + 1);
					toSendPacket.AddData(associatedAccount.ReverseList.Count);
					toSendPacket.AddData(associatedAccount.ReverseList[i].EmailAddress);
					toSendPacket.AddData(associatedAccount.ReverseList[i].DisplayName);
					associatedClient.TcpClientHandler.SendData(toSendPacket.ToString() + Environment.NewLine);
				}
			}
		}
			
		/// <summary>
		/// Updates the status of a contact
		/// </summary>
		public void UpdateContactStatus(MSNUser contact, int transactionID = 0)
		{
			if (contact.Status == MSNPStatus.OFFLINE || contact.Status == MSNPStatus.APPEAR_OFFLINE)
			{
				MSNPPacket toSendPacket = MSNPPacket.CreateEmptyPacket();
				toSendPacket.PacketCommand = "FLN";
				toSendPacket.AddData(contact.EmailAddress);
				associatedClient.TcpClientHandler.SendData(toSendPacket.ToString() + Environment.NewLine);
			}
			else
			{
				MSNPPacket toSendPacket = MSNPPacket.CreateEmptyPacket();
				toSendPacket.PacketCommand = "NLN";
				toSendPacket.AddData(Tools.StrFromStatus(contact.Status));
				toSendPacket.AddData(contact.EmailAddress);
				toSendPacket.AddData(contact.DisplayName);
				associatedClient.TcpClientHandler.SendData(toSendPacket.ToString() + Environment.NewLine);
			}
		}

		/// <summary>
		/// Updates the statuses of the contacts
		/// </summary>
		public void UpdateContactsStatuses(int transactionID = 0)
		{
			foreach (MSNUser contact in associatedAccount.AllowedList.ToArray())
			{
				UpdateContactStatus(contact, transactionID);
			}
		}
	}
}