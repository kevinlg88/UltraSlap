using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class HostBroadcaster : MonoBehaviour
{
    UdpClient udpClient;
    IPEndPoint broadcastEP;
    string ip;

    void Start()
    {
        udpClient = new UdpClient();
        udpClient.EnableBroadcast = true;
        broadcastEP = new IPEndPoint(IPAddress.Broadcast, 54321);

        ip = GetLocalIPAddress();
        InvokeRepeating(nameof(BroadcastIP), 1f, 1f);
    }

    void BroadcastIP()
    {
        string message = "HOST:" + ip;
        byte[] data = Encoding.UTF8.GetBytes(message);
        udpClient.Send(data, data.Length, broadcastEP);
    }

    string GetLocalIPAddress()
    {
        foreach (var ni in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
        {
            if (ni.AddressFamily == AddressFamily.InterNetwork)
                return ni.ToString();
        }
        return "127.0.0.1";
    }

    void OnApplicationQuit()
    {
        udpClient?.Close();
    }
}