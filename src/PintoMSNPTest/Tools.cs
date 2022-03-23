using System;
using PintoMSNPTest.MSNClasses;
using PintoMSNPTest.MSNPClasses;

namespace PintoMSNPTest
{
	/// <summary>
	/// Tools for the server
	/// </summary>
	public static class Tools
	{
		/// <summary>
		/// Gets an MSNP status from a string
		/// </summary>
		public static MSNPStatus StatusFromStr(string str)
		{
			switch (str)
			{
				case "NLN":
					return MSNPStatus.ONLINE;
				case "AWY":
					return MSNPStatus.AWAY;
				case "IDL":
					return MSNPStatus.IDLE;
				case "BRB":
					return MSNPStatus.BE_RIGHT_BACK;
				case "LUN":
					return MSNPStatus.OUT_TO_LUNCH;
				case "BSY":
					return MSNPStatus.BUSY;
				case "PHN":
					return MSNPStatus.ON_THE_PHONE;
				case "HDN":
					return MSNPStatus.APPEAR_OFFLINE;
				case "FLN":
					return MSNPStatus.OFFLINE;
				default:
					return MSNPStatus.OFFLINE;
			}
		}

		/// <summary>
		/// Gets a string from an MSNP status
		/// </summary>
		public static string StrFromStatus(MSNPStatus status)
		{
			switch (status)
			{
				case MSNPStatus.ONLINE:
					return "NLN";
				case MSNPStatus.AWAY:
					return "AWY";
				case MSNPStatus.IDLE:
					return "IDL";
				case MSNPStatus.BE_RIGHT_BACK:
					return "BRB";
				case MSNPStatus.OUT_TO_LUNCH:
					return "LUN";
				case MSNPStatus.BUSY:
					return "BSY";
				case MSNPStatus.ON_THE_PHONE:
					return "PHN";
				case MSNPStatus.APPEAR_OFFLINE:
					return "HDN";
				case MSNPStatus.OFFLINE:
					return "FLN";
				default:
					return "FLN";
			}
		}

		/// <summary>
		/// Gets a string representation of an MSNP error code
		/// </summary>
		public static string ErrCodeToStr(MSNPErrorCode code)
		{
			return code.ToString().Replace("CODE_", string.Empty);
		}
	}
}