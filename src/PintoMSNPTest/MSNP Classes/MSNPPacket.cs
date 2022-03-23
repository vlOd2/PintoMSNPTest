using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PintoMSNPTest.MSNPClasses
{
	/// <summary>
	/// A class that represents an MSNP packet
	/// </summary>
	public class MSNPPacket
	{
		public string PacketCommand = "";
		public int PacketTransactionID = -1;
		private List<object> packetData = new List<object>();

		/// <summary>
		/// A class that represents an MSNP packet
		/// </summary>
		private MSNPPacket() {}

		/// <summary>
		/// Creates an empty packet
		/// </summary>
		public static MSNPPacket CreateEmptyPacket()
		{
			MSNPPacket newPacket = new MSNPPacket();
			newPacket.PacketCommand = "500";
			return newPacket;
		}

		/// <summary>
		/// Creates a packet from raw decoded data
		/// </summary>
		public static MSNPPacket CreatePacketFromParsedData(string rawDecodedData)
		{
			MSNPPacket newPacket = new MSNPPacket();
			string[] parsedData = rawDecodedData.Split((char)0x20);

			int packetPieceIndex = 0;
			foreach (string packetPiece in parsedData)
			{
				if (packetPieceIndex == 0)
				{
					newPacket.PacketCommand = packetPiece;
				}
				else if (packetPieceIndex == 1)
				{
					int packetTransactionID;

					if (Int32.TryParse(packetPiece, out packetTransactionID))
					{
						newPacket.PacketTransactionID = packetTransactionID;
					}
					else
					{
						newPacket.AddData(packetPiece);
					}
				}
				else
				{
					newPacket.AddData(packetPiece);
				}
					
				packetPieceIndex++;
			}

			return newPacket;
		}

		/// <summary>
		/// Adds data into this packet
		/// </summary>
		public void AddData(object data)
		{
			packetData.Add(data);
		}

		/// <summary>
		/// Removes data from this packet
		/// </summary>
		public void RemoveData(string data)
		{
			if (!packetData.Contains(data))
			{
				throw new InvalidOperationException("The specified data doesn't exist in this packet.");
			}
			else
			{
				packetData.Remove(data);
			}
		}

		/// <summary>
		/// Gets data from an index from this packet
		/// </summary>
		public object GetData(int dataIndex)
		{
			try 
			{
				return packetData[dataIndex];
			}
			catch 
			{
				return null;
			}
		}

		/// <summary>
		/// Returns a string that represents this MSNP packet
		/// </summary>
		public override string ToString()
		{
			string finalString;
			if (PacketTransactionID != -1)
				finalString = PacketCommand + " " + PacketTransactionID + " ";
			else
				finalString = PacketCommand + " ";

			foreach (object packetDataEntry in packetData)
			{
				if (packetDataEntry == null)
					finalString += "-" + " ";
				else
					finalString += packetDataEntry.ToString() + " ";
			}

			if (finalString.EndsWith(((char)0x20).ToString()))
				finalString = finalString.TrimEnd(new char[] { (char)0x20 });

			return finalString;
		}

		/// <summary>
		/// Returns a JSON string that represents this MSNP packet
		/// </summary>
		public string ToJSONString()
		{
			JObject finalObject = new JObject();
			JObject packetObject = new JObject();

			finalObject.Add("Packet", packetObject);
			packetObject.Add("Command", JToken.FromObject(PacketCommand));
			packetObject.Add("TransactionID", JToken.FromObject(PacketTransactionID));
			packetObject.Add("Arguments", JToken.FromObject(new object[0]));

			foreach (string packetDataEntry in packetData)
			{
				((JArray)packetObject.GetValue("Arguments")).Add(packetDataEntry);
			}

			return JsonConvert.SerializeObject(finalObject, Formatting.Indented);
		}
	}
}

