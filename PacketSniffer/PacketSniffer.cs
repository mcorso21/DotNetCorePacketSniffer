using PacketSniffer.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

using PacketSniffer.Parsers;

namespace PacketSniffer
{
    public static class PacketSniffer
    {
        private static Socket socket;
        private static byte[] packetData = new byte[65535];
        private static bool parsePackets;

        /// <summary>
        /// 
        ///     Gets a list of all of the Network Interfaces on the system and their associated IP Address.
        ///     Filters out addresses that are NOT IPv4
        /// 
        /// </summary>
        /// <returns>
        /// 
        ///     Returns a Dicitonary containing all IPv4 Network Interfaces and their IP Address. The values are stored as:
        ///         { Interface_Display_Name, IP_Address }
        /// 
        /// </returns>
        public static Dictionary<string, IPAddress> GetIpv4NetworkInterfaces()
        {
            try
            {
                // Storing network interfaces as: { Name, IPAddress}
                Dictionary<string, IPAddress> ipv4Interfaces = new Dictionary<string, IPAddress>();

                var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

                foreach (var _interface in networkInterfaces)
                {
                    var _interfaceAddresses = _interface.GetIPProperties().UnicastAddresses;
                    
                    foreach (var addr in _interfaceAddresses)
                    {
                        // Only store IPv4 interfaces
                        if (addr.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            string interfaceName = _interface.Name;
                            IPAddress interfaceIp = addr.Address;
                            ipv4Interfaces.Add(interfaceName, interfaceIp);
                        }
                    }
                }

                return ipv4Interfaces;
            }
            catch (Exception ex)
            {
                throw new Exception($"GetIpv4NetworkInterfaces > {ex.Message}");
            }
        }


        /// <summary>
        /// 
        ///     Creates a socket to listen to all traffic on the specified IP Address
        ///     
        /// </summary>
        public static void CreateSocket(IPAddress socketIp)
        {
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP);
                socket.Bind(new IPEndPoint(socketIp, 0));
                socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, true);
                socket.IOControl(IOControlCode.ReceiveAll, BitConverter.GetBytes(1), BitConverter.GetBytes(1));
                socket.Blocking = false;
            }
            catch (Exception ex)
            {
                throw new Exception($"CreateSocket > {ex.Message}");
            }
        }

        /// <summary>
        /// 
        ///     Begins the receive operation on the socket.
        ///     Takes a callback Action that will receive the parsed packet
        /// 
        /// </summary>
        public static void StartSniffer(Action<IPv4PacketModel> callback)
        {
            try
            {
                parsePackets = true;

                Action<IAsyncResult> OnPacketReceive = null;
                OnPacketReceive = (asyncResult) =>
                {
                    try
                    {
                        if (parsePackets)
                        {
                            IPv4PacketModel ipv4PacketModel = IPv4PacketParser.ParseIpv4Packet(packetData);
                            callback(ipv4PacketModel);

                            socket.BeginReceive(packetData, 0, packetData.Length, SocketFlags.None, new AsyncCallback(OnPacketReceive), null);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"OnPacketReceive > {ex.Message}");
                    }
                };

                socket.BeginReceive(packetData, 0, packetData.Length, SocketFlags.None, new AsyncCallback(OnPacketReceive), null);
            }
            catch (Exception ex)
            {
                throw new Exception($"StartSniffer > {ex.Message}");
            }
        }

        public static void StopSniffer()
        {
            parsePackets = false;
        }
    }
}
