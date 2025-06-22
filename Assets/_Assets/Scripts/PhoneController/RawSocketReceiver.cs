using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class RawSocketReceiver : MonoBehaviour
{
    private Socket socket;
    private Thread receiveThread;
    private EndPoint remoteEndPoint;
    private byte[] buffer = new byte[1024];
    private bool isRunning = false;

    void Start()
    {
        try
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(new IPEndPoint(IPAddress.Any, 12345));
            remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

            isRunning = true;
            receiveThread = new Thread(ReceiveLoop);
            receiveThread.IsBackground = true;
            receiveThread.Start();

            Debug.Log("Raw UDP socket listening on port 12345");
        }
        catch (Exception e)
        {
            Debug.LogError("Socket setup failed: " + e.Message);
        }
    }

    private void ReceiveLoop()
    {
        while (isRunning)
        {
            try
            {
                int length = socket.ReceiveFrom(buffer, ref remoteEndPoint);
                string message = Encoding.UTF8.GetString(buffer, 0, length);
                Debug.Log($"[RAW RECEIVED] {remoteEndPoint}: {message}");
            }
            catch (SocketException e)
            {
                Debug.LogError("Socket receive error: " + e.Message);
            }
            catch (Exception e)
            {
                Debug.LogError("ReceiveLoop error: " + e.Message);
            }
        }
    }

    void OnApplicationQuit()
    {
        isRunning = false;
        socket?.Close();
        receiveThread?.Abort();
    }
}