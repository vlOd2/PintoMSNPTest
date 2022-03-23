using System;
using PintoMSNPTest.MSNClasses;

namespace PintoMSNPTest.MSNPClasses
{
	/// <summary>
	/// A class that handles events for an MSNP client
	/// </summary>
	public class MSNPClientEventsManager
	{
		private MSNPClient associatedClient;

		/// <summary>
		/// A class that handles events for an MSNP client
		/// </summary>
		public MSNPClientEventsManager(MSNPClient associatedClient)
		{
			this.associatedClient = associatedClient;
		}


		/// <summary>
		/// Function called when a contact request is received
		/// </summary>
		public void OnRecvContactRequest(MSNContact contact)
		{

		}

		/// <summary>
		/// Function called when a list has been updated
		/// </summary>
		public void OnListUpdate(ListType listType, MSNContact affectedContact, bool removed = false)
		{
			if (associatedClient.ClientSync.IsDoingSync)
				return;

			switch (listType)
			{
				case ListType.FORWARD:
					break;
				case ListType.ALLOWED:
					break;
				case ListType.BLOCKED:
					break;
				case ListType.REVERSE:
					break;
				default:
					break;
			}
		}

		/// <summary>
		/// Function called when a contact has been updated
		/// </summary>
		public void OnContactUpdate(ContactUpdateType updateType, MSNContact contact)
		{
			if (contact == associatedClient.AssociatedAccount.ContactEntry)
				return;

			associatedClient.ClientSync.UpdateContactStatus(contact);
		}
	}
}