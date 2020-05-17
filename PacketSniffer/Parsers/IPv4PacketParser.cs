using System;
using System.Net;

using PacketSniffer.Models;
using PacketSniffer.Shared;

namespace PacketSniffer.Parsers
{
    public static class IPv4PacketParser
    {
        public static IPv4PacketModel ParseIpv4Packet(byte[] packet)
        {
            try
            {
                IPv4PacketModel ipv4PacketModel = new IPv4PacketModel()
                {
                    SourceIP = new IPAddress(Tools.GetSubArray(packet, 12, 4)),
                    SourcePort = 0,
                    DestinationIP = new IPAddress(Tools.GetSubArray(packet, 16, 4)),
                    DestinationPort = 0,
                    PacketSizeInBytes = BitConverter.ToUInt16(Tools.GetSubArray(packet, 2, 2)),
                    Protocol = Convert.ToUInt16(packet[9]),
                    ProtocolAsString = "Other"
                };

                // If protocol is TCP or UDP
                if (ipv4PacketModel.Protocol == 6 || ipv4PacketModel.Protocol == 17)
                {
                    // Get protocol display name
                    ipv4PacketModel.ProtocolAsString = (ipv4PacketModel.Protocol == 6) ? "TCP" : "UDP";

                    // Get IPv4 header length to parse child packet
                    int ipv4HeaderLengthInBytes = 4 * Convert.ToUInt16(packet[0] & 0x0F);

                    ipv4PacketModel.SourcePort = BitConverter.ToUInt16(Tools.GetSubArray(packet, ipv4HeaderLengthInBytes, 2));
                    ipv4PacketModel.DestinationPort = BitConverter.ToUInt16(Tools.GetSubArray(packet, (ipv4HeaderLengthInBytes + 2), 2));

                    // Get child packet header length to parse packet data
                    int childPacketHeaderLengthInBytes = 8; // UDP header is always 8 bytes
                    if (ipv4PacketModel.Protocol == 6)
                    {
                        childPacketHeaderLengthInBytes = 4 * (Convert.ToUInt16(packet[(ipv4HeaderLengthInBytes + 12)]) >> 4);
                    }
                    int dataStart = ipv4HeaderLengthInBytes + childPacketHeaderLengthInBytes;

                    // Sometimes total packet length is set to 0
                    if (ipv4PacketModel.PacketSizeInBytes - dataStart > 0)
                        ipv4PacketModel.Data = Tools.GetSubArray(packet, dataStart, (ipv4PacketModel.PacketSizeInBytes - dataStart));
                }

                return ipv4PacketModel;
            }
            catch (Exception ex)
            {
                throw new Exception($"ParseIpv4Packet > {ex.Message}");
            }
        }
    }
}
