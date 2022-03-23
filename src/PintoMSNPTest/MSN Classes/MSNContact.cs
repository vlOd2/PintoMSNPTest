using System;
using PintoMSNPTest.MSNPClasses;

namespace PintoMSNPTest.MSNClasses
{
	/// <summary>
	/// A class that represents an MSN contact
	/// </summary>
	public class MSNContact : MSNUser
	{
		private MSNAccount associatedAccount;

		/// <summary>
		/// The account associated with this contact instance
		/// </summary>
		public MSNAccount AssociatedAccount { get { return associatedAccount; } }

		/// <summary>
		/// A class that represents an MSN contact
		/// </summary>
		public MSNContact(MSNAccount associatedAccount, string emailAddress, string displayName = null) : base(emailAddress, displayName) 
		{
			this.associatedAccount = associatedAccount;
			StatusChangedEvent += OnStatusChanged;
		}

		public MSNContact ConvertToNewAccount(MSNAccount newAccount)
		{
			MSNContact newContact = new MSNContact(newAccount, EmailAddress, DisplayName);
			associatedAccount.ClonesOfContactEntry.Add(newContact);
			return newContact;
		}

		private void OnStatusChanged(object sender, EventArgs e)
		{
			associatedAccount.AssociatedClient.ClientEventsManager.OnContactUpdate(ContactUpdateType.STATUS, this);
		}
	}
}