using System;
using PintoMSNPTest.MSNPClasses;

namespace PintoMSNPTest.MSNClasses
{
	/// <summary>
	/// A class that represents an MSN user
	/// </summary>
	public class MSNUser
	{
		private MSNPStatus status = MSNPStatus.OFFLINE;

		/// <summary>
		/// The email address associated with this user
		/// </summary>
		public readonly string EmailAddress;
		/// <summary>
		/// The display name associated with this user
		/// </summary>
		public string DisplayName;
		/// <summary>
		/// The status associated with this user
		/// </summary>
		public virtual MSNPStatus Status 
		{
			get 
			{
				return status;
			}
			set 
			{
				status = value;
				if (StatusChangedEvent != null)
					StatusChangedEvent.Invoke(this, EventArgs.Empty);
			}
		}
		/// <summary>
		/// Event fired when the current status has changed
		/// </summary>
		public event EventHandler StatusChangedEvent;

		/// <summary>
		/// A class that represents an MSN user
		/// </summary>
		public MSNUser(string emailAddress, string displayName = null)
		{
			EmailAddress = emailAddress;
			DisplayName = (displayName != null) ? displayName : emailAddress;
		}
	}
}