## PacketSniffer

---

### Overview

---

- This is a basic packet sniffer written in **.NET CORE 3.1** that listens to all packets on the specific network interface. While it gathers general information about all packet types (see below for specifics), it only parses data from TCP and UDP packets.

- The default output of a parsed packet is:

  - Fields for all packet types:

    - _DateTime_ **TimeReceived**
    - _IPAddress_ **SourceIP**
    - _IPAddress_ **DestinationIP**
    - _UInt16_ **PacketSizeInBytes**
    - _UInt16_ **Protocol**
    - _string_ **ProtocolAsString** (Either 'TCP', 'UDP', or 'Other')

  - Fields only parsed for TCP/UDP
    - _UInt16_ **SourcePort**
    - _UInt16_ **DestinationPort**
    - _byte[]_ **Data**

### Setup

---

- **The program must be run as an Administrator on Windows to work properly.**

- The out of the box the parser will create a console that asks the user which network interface to listen to and, once selected, will parse and output summaries of packets received.
- The core of the parser only requires the IPAddress of the network interface to attach to. From there, the parser will create IPv4PacketModel objects for every packet receive.
  - My method for obtaining the IPAddress from the user can be found at Program.PromptUserForNetworkInterface
- You can create a custom callback and pass it to **PacketSniffer.StartSniffer** to do whatever you want with the packet data.
  - My callback Action that receives the parsed packets can be found at **Program.callback**
