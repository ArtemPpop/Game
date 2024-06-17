using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Server
{
    private static List<Socket> clients = new List<Socket>();
    private static Dictionary<Socket, (double x, double y)> playerPositions = new Dictionary<Socket, (double x, double y)>();
    private static readonly int port = 8080;

    static void Main(string[] args)
    {
        Console.WriteLine("Starting server...");
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
        Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        listener.Bind(endPoint);
        listener.Listen(10);

        Console.WriteLine("Server started. Waiting for connections...");

        while (true)
        {
            Socket clientSocket = listener.Accept();
            clients.Add(clientSocket);
            playerPositions[clientSocket] = (1, 1); // Initial position for new player
            Console.WriteLine("Client connected.");

            Thread clientThread = new Thread(() => HandleClient(clientSocket));
            clientThread.Start();
        }
    }

    private static void HandleClient(Socket clientSocket)
    {
        byte[] buffer = new byte[1024];

        while (true)
        {
            try
            {
                int bytesRead = clientSocket.Receive(buffer);
                if (bytesRead > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    string[] parts = message.Split(',');

                    double x = double.Parse(parts[0]);
                    double y = double.Parse(parts[1]);

                    playerPositions[clientSocket] = (x, y);

                    BroadcastPlayerPositions();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                clients.Remove(clientSocket);
                playerPositions.Remove(clientSocket);
                clientSocket.Close();
                break;
            }
        }
    }

    private static void BroadcastPlayerPositions()
    {
        StringBuilder positions = new StringBuilder();

        foreach (var player in playerPositions)
        {
            positions.Append($"{player.Value.x},{player.Value.y};");
        }

        byte[] data = Encoding.UTF8.GetBytes(positions.ToString());

        foreach (var client in clients)
        {
            try
            {
                client.Send(data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                clients.Remove(client);
                playerPositions.Remove(client);
                client.Close();
                break;
            }
        }
    }
}
