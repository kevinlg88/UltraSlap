using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class InputReceiver : MonoBehaviour
{
    UdpClient udpClient;
    Thread receiveThread;
    public Dictionary<string, PlayerInputData> playerInputs = new();

    private Dictionary<string, float> lastLogTime = new();
    private const float logCooldown = 1f;

    void Start()
    {
        udpClient = new UdpClient(12345);
        receiveThread = new Thread(ReceiveData) { IsBackground = true };
        receiveThread.Start();
    }

    void ReceiveData()
    {
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
        while (true)
        {
            try
            {
                byte[] data = udpClient.Receive(ref remoteEP);
                string message = Encoding.UTF8.GetString(data);
                string[] parts = message.Split(',');
                Debug.Log($"[RAW] From {remoteEP.Address}:{remoteEP.Port} ? {message}");

                if (parts.Length < 5) continue;

                string id = parts[0];
                float x = float.Parse(parts[1]);
                float y = float.Parse(parts[2]);
                bool action1 = parts[3] == "1";
                bool action2 = parts[4] == "1";

                var input = new PlayerInputData
                {
                    movement = new Vector2(x, y),
                    action1 = action1,
                    action2 = action2
                };

                lock (playerInputs)
                {
                    playerInputs[id] = input;

                    float now = Time.realtimeSinceStartup;

                    if (!lastLogTime.TryGetValue(id, out float lastTime) || now - lastTime > logCooldown)
                    {
                        lastLogTime[id] = now;
                        Debug.Log($"[InputReceiver] Player '{id}': Move({x:F2},{y:F2}) A1: {action1} A2: {action2}");
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("[InputReceiver] Receive error: " + e.Message);
            }
        }
    }

    public PlayerInputData GetPlayerInput(string id)
    {
        lock (playerInputs)
        {
            return playerInputs.TryGetValue(id, out var input) ? input : new PlayerInputData();
        }
    }

    void OnApplicationQuit()
    {
        receiveThread?.Abort();
        udpClient?.Close();
    }
}

public struct PlayerInputData
{
    public Vector2 movement;
    public bool action1;
    public bool action2;
}