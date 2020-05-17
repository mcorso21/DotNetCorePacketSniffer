using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using PacketSniffer.Models;
using PacketSniffer.Shared;

namespace PacketSniffer
{
    class Program
    {
        /// <summary>
        /// 
        /// *** Customize this Action to do what you want with the packet data ***
        /// 
        /// The parser creates an IPv4PacketModel for each packet received and then sends it to the callback (this) provided in the parameters
        /// The callback is passed to the sniffer in Main below
        /// 
        /// The below implementation simply displays the packet summary in an easily readable format (ie source, destination, size, truncated data as ASCII, etc.)
        /// 
        /// </summary>
        // StartSniffer expects a callback to call with the IPv4PacketModel
        static Action<IPv4PacketModel> callback = (IPv4PacketModel packet) =>
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"{packet.TimeReceived.ToString("HH:mm:ss")} | ");

            sb.Append($"{packet.SourceIP.ToString().PadLeft(15)}:{packet.SourcePort.ToString().PadLeft(5)}");
            sb.Append($" ---{packet.ProtocolAsString}---> ");
            sb.Append($"{packet.DestinationIP.ToString().PadLeft(15)}:{packet.DestinationPort.ToString().PadLeft(5)}");

            double packetSizeInKb = ((double)packet.PacketSizeInBytes / (double)1000);
            sb.Append($" | {Math.Round(packetSizeInKb, 1).ToString("00.0")} kB | ");

            byte[] truncatedData = Tools.GetSubArray(packet.Data, 0, 40);
            string truncatedDataAsAscii = Tools.ConvertByteArrayToAscii(truncatedData);
            truncatedDataAsAscii = Regex.Replace(truncatedDataAsAscii, @"\t|\n|\r", "");

            sb.Append($"{truncatedDataAsAscii}");

            Console.WriteLine(sb.ToString());
        };


        static void Main(string[] args)
        {
            // Get a list of all of the IPv4 interfaces on the host, stored as: 
            //  { Interface_Display_Name, Interface_IP_Address }
            Dictionary<string, IPAddress> networkInterfaces = PacketSniffer.GetIpv4NetworkInterfaces();

            // Simple way of having the user select the Network Interface / IP Address
            IPAddress targetIp = PromptUserForNetworkInterface(networkInterfaces);

            // Create a socket listening to the above IP Address
            PacketSniffer.CreateSocket(targetIp);

            // Start sniffer with the specified callback
            PacketSniffer.StartSniffer(callback);

            Console.ReadLine();
        }

        /// <summary>
        /// 
        ///     Ask the user which Network Interface / IP Address to sniff
        /// 
        /// </summary>
        /// <returns>
        /// 
        ///     The IP address to sniff
        ///     
        /// </returns>
        private static IPAddress PromptUserForNetworkInterface(Dictionary<string, IPAddress> networkInterfaces)
        {
            try
            {
                var networkInterfaceNames = networkInterfaces.Keys.ToList();

                int selection = -1;

                while (selection < 0 || selection >= networkInterfaceNames.Count())
                {
                    Console.WriteLine($"Please select the network interface to listen to:");

                    for (int i = 0; i < networkInterfaceNames.Count; i++)
                    {
                        Console.WriteLine($"\t{i}: {networkInterfaceNames[i]}");
                    }

                    string input = Console.ReadLine();

                    try { selection = Int32.Parse(input); }
                    catch (Exception) { }
                }

                IPAddress selectedIp = networkInterfaces[networkInterfaceNames[selection]];
                return selectedIp;
            }
            catch (Exception ex)
            {
                throw new Exception($"PromptUserForNetworkInterface > {ex.Message}");
            }
        }
    }
}
