using System;
using System.Net;

public static partial class MyProtocol
{
    private const int SizeUint = sizeof(uint);

    public static void Serialize(uint value, byte[] target, ref int offset)
    {
        int host = (int)value;
        uint network = (uint)IPAddress.HostToNetworkOrder(host);
        byte[] bytes = BitConverter.GetBytes(network);
        Buffer.BlockCopy(bytes, 0, target, offset, SizeUint);
        offset += SizeUint;
    }

    public static void Deserialize(out uint value, byte[] source, ref int offset)
    {
        int host = BitConverter.ToInt32(source, offset);
        int network = IPAddress.NetworkToHostOrder(host);
        value = (uint)network;
        offset += SizeUint;
    }
}