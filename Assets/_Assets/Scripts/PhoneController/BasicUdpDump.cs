using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class BasicUdpDump : MonoBehaviour
{
    private UdpClient udpClient;
    private IPEndPoint remoteEP;

    void Start()
    {
        try
        {
            remoteEP = new IPEndPoint(IPAddress.Any, 12345);
            udpClient = new UdpClient(AddressFamily.InterNetwork);
            udpClient.Client.Bind(remoteEP);

            Debug.Log("UDP listening on port 12345");

            udpClient.BeginReceive(ReceiveCallback, null);
        }
        catch (Exception e)
        {
            Debug.LogError("Socket bind failed: " + e.Message);
        }
    }

    void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            byte[] data = udpClient.EndReceive(ar, ref remoteEP);
            string message = Encoding.UTF8.GetString(data);
            Debug.Log($"[RECEIVED] {remoteEP.Address}: {message}");

            udpClient.BeginReceive(ReceiveCallback, null);
        }
        catch (Exception e)
        {
            Debug.LogError("ReceiveCallback error: " + e.Message);
        }
    }

    void OnApplicationQuit()
    {
        udpClient?.Close();
    }
}