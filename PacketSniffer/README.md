## PacketSniffer

### Overview
- This is a basic packet sniffer written in .NET CORE 3.1 that listens to all packets on the specific network interface. While it gathers general information about all packet types (see below for specifics), it only parses data from TCP and UDP packets.

- The default formatted output of a parsed packet is:
    
    - Fields for all packet types:
        - DateTime TimeReceived
        - IPAddress SourceIP
        - IPAddress DestinationIP
        - UInt16 PacketSizeInBytes
        - UInt16 Protocol
        - string ProtocolAsString (Either 'TCP', 'UDP', or 'Other')
    
    - Fields only parsed for TCP/UDP
        - UInt16 SourcePort
        - UInt16 DestinationPort
        - byte[] Data


### Setup
- The program must be run as an Administrator to work properly.
- Out of the box the parser will create a console and ask the user which network interface to listen to and, once selected, will parse and output summaries of packets received.
- The core of the parser only requires the IPAddress of the network interface to attach to. From there, the parser will create IPv4PacketModel objects for every packet receive.
    - My method for obtaining the IPAddress from the user is in Program.PromptUserForNetworkInterface
- You can create a custom callback and pass it to PacketSniffer.StartSniffer to do whatever you want with the packet data.
    - My callback Action is written in Program.callback
