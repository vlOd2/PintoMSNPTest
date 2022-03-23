using System;
using System.Collections.Generic;
using PintoMSNPTest.MSNPClasses;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace PintoMSNPTest.MSNClasses
{
	/// <summary>
	/// A class that represents an MSN account
	/// </summary>
	public class MSNAccount : MSNUser
	{
		/// <summary>
		/// The MSNP client associated with this account
		/// </summary>
		public MSNPClient AssociatedClient;
		/// <summary>
		/// An instance of an MSN contact that represents this account
		/// </summary>
		public MSNContact ContactEntry;
		/// <summary>
		/// The list of clones of the contact entry
		/// </summary>
		public readonly List<MSNContact> ClonesOfContactEntry = new List<MSNContact>();
		/// <summary>
		/// The forward list associated with this account
		/// </summary>
		public readonly ObservableCollection<MSNContact> ForwardList = new ObservableCollection<MSNContact>();
		/// <summary>
		/// The allowed list associated with this account
		/// </summary>
		public readonly ObservableCollection<MSNContact> AllowedList = new ObservableCollection<MSNContact>();
		/// <summary>
		/// The blocked list associated with this account
		/// </summary>
		public readonly ObservableCollection<MSNContact> BlockedList = new ObservableCollection<MSNContact>();
		/// <summary>
		/// The reverse list associated with this account
		/// </summary>
		public readonly ObservableCollection<MSNContact> ReverseList = new ObservableCollection<MSNContact>();
		/// <summary>
		/// Value indicating whether the user allows messages from non-contacts
		/// </summary>
		public bool MsgPrivacy = false;
		/// <summary>
		/// Value indicating whether the user should be alerted when an user adds them
		/// </summary>
		public bool ContactPrivacy = false;
		/// <summary>
		/// The status associated with this account
		/// </summary>
		public override MSNPStatus Status
		{
			get
			{
				return base.Status;
			}
			set
			{
				ChangeStatus(value);
			}
		}

		/// <summary>
		/// A class that represents an MSN account
		/// </summary>
		public MSNAccount(MSNPClient associatedClient, string emailAddress, string displayName = null) : base(emailAddress, displayName) 
		{
			AssociatedClient = associatedClient;
			ContactEntry = new MSNContact(this, emailAddress, displayName);

			ForwardList.CollectionChanged += ForwardList_CollectionChanged;
			AllowedList.CollectionChanged += AllowedList_CollectionChanged;
			BlockedList.CollectionChanged += BlockedList_CollectionChanged;
			ReverseList.CollectionChanged += ReverseList_CollectionChanged;
		}

		/// <summary>
		/// Adds a contact to the forward and allow lists
		/// </summary>
		public void AddContact(MSNContact newContact)
		{
			foreach (MSNContact contact in ForwardList.ToArray())
			{
				if (contact.EmailAddress.Equals(newContact.EmailAddress))
				{
					RemoveContact(contact);
					break;
				}	
			}

			foreach (MSNContact contact in AllowedList.ToArray())
			{
				if (contact.EmailAddress.Equals(newContact.EmailAddress))
				{
					RemoveContact(contact);
					break;
				}	
			}

			ForwardList.Add(newContact.ConvertToNewAccount(this));
			newContact.AssociatedAccount.ReverseList.Add(ContactEntry.ConvertToNewAccount(newContact.AssociatedAccount));
		}

		/// <summary>
		/// Removes a contact from the forward and allow lists
		/// </summary>
		public void RemoveContact(MSNContact existingContact)
		{
			if (ForwardList.Contains(existingContact))
				ForwardList.Remove(existingContact);
			if (AllowedList.Contains(existingContact))
				AllowedList.Remove(existingContact);
		}

		/// <summary>
		/// Changes the current status of this MSN account
		/// </summary>
		private void ChangeStatus(MSNPStatus newStatus)
		{
			base.Status = newStatus;
			ContactEntry.Status = newStatus;

			foreach (MSNContact clone in ClonesOfContactEntry.ToArray())
			{
				clone.Status = ContactEntry.Status;
				clone.DisplayName = ContactEntry.DisplayName;
			}
		}

		/// <summary>
		/// Collection changed event for the forward list
		/// </summary>
		private void ForwardList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (MSNContact addedContact in e.NewItems)
				{
					AssociatedClient.ClientEventsManager.OnListUpdate(ListType.FORWARD, addedContact);
				}
			}
			else if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				foreach (MSNContact removedContact in e.OldItems)
				{
					AssociatedClient.ClientEventsManager.OnListUpdate(ListType.FORWARD, removedContact, true);
				}
			}
		}

		/// <summary>
		/// Collection changed event for the forward list
		/// </summary>
		private void AllowedList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (MSNContact addedContact in e.NewItems)
				{
					AssociatedClient.ClientEventsManager.OnListUpdate(ListType.ALLOWED, addedContact);
				}
			}
			else if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				foreach (MSNContact removedContact in e.OldItems)
				{
					AssociatedClient.ClientEventsManager.OnListUpdate(ListType.ALLOWED, removedContact, true);
				}
			}
		}

		/// <summary>
		/// Collection changed event for the forward list
		/// </summary>
		private void BlockedList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (MSNContact addedContact in e.NewItems)
				{
					AssociatedClient.ClientEventsManager.OnListUpdate(ListType.BLOCKED, addedContact);
				}
			}
			else if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				foreach (MSNContact removedContact in e.OldItems)
				{
					AssociatedClient.ClientEventsManager.OnListUpdate(ListType.BLOCKED, removedContact, true);
				}
			}
		}

		/// <summary>
		/// Collection changed event for the forward list
		/// </summary>
		private void ReverseList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (MSNContact addedContact in e.NewItems)
				{
					AssociatedClient.ClientEventsManager.OnListUpdate(ListType.REVERSE, addedContact);
				}
			}
			else if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				foreach (MSNContact removedContact in e.OldItems)
				{
					AssociatedClient.ClientEventsManager.OnListUpdate(ListType.REVERSE, removedContact, true);
				}
			}
		}
	}
}