using System;
using System.Net;

namespace PacketSniffer.Models
{
    public class IPv4PacketModel
    {
        public readonly DateTime TimeReceived;
        public IPAddress SourceIP, DestinationIP;
        public UInt16 Protocol, PacketSizeInBytes, SourcePort, DestinationPort;
        public string ProtocolAsString;
        public byte[] Data;

        public IPv4PacketModel()
        {
            this.TimeReceived = DateTime.Now;
        }
    }
}
