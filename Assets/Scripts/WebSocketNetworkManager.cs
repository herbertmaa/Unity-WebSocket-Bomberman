
using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class WebSocketNetworkManager : NetworkManager
{

    private ClientWebSocket mySocket = null;
    //private string url = "ws://localhost:5000";
    private string url = "ws://ec2-54-152-1-19.compute-1.amazonaws.com";
    // Start is called before the first frame update

    public override async void Connect()
    {
        using (ClientWebSocket socket = new ClientWebSocket())
        {
            Debug.Log("Connecting to web socket URL: " + url);
            await socket.ConnectAsync(new Uri(url), CancellationToken.None);
            Debug.Log("Successfully connected to web socket");

            mySocket = socket; // Assign a reference to this ClientWebSocket

            BroadCastMessage("new_connection");
            await ReceiveData(socket);
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "close", CancellationToken.None);
        }
    }


    public async Task ReceiveData(ClientWebSocket socket)
    {
        var buffer = new ArraySegment<byte>(new byte[2048]);
        do
        {
            WebSocketReceiveResult result;
            using (var ms = new MemoryStream())
            {
                do
                {
                    result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                    ms.Write(buffer.Array, buffer.Offset, result.Count);
                } while (!result.EndOfMessage);

                if (result.MessageType == WebSocketMessageType.Close)
                    break;


                ms.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(ms, Encoding.UTF8))
                {
                    string message = await reader.ReadToEndAsync();
                    HandleRequest(message);
                }
            }
        } while (true);
    }


    public override void BroadCastMessage(string message)
    {
        ArraySegment<byte> myArrSegAll = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
        mySocket.SendAsync(myArrSegAll, WebSocketMessageType.Text, true, CancellationToken.None);
    }

    public override void HandleRequest(string request)
    {
        Debug.Log(request);
        request = request.Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
        request = Regex.Replace(request, @"[^\u0009\u000A\u000D\u0020-\u007E]", "");

        string[] request_params = request.Split(' ');
        if (request.Contains("new_connection"))
        {

            string unique_id = request_params[request_params.Length - 1];
            float x = float.Parse(request_params[1]);
            float y = float.Parse(request_params[2]);
            playerSpawner.SpawnPlayer(x, y);
            string message = "connect" + " " + x + " " + y;
            BroadCastMessage(message);
        }
        else if (request.Contains("disconnect") && enemySpawner != null)
        {
            string unique_id = request_params[request_params.Length - 1];
            enemySpawner.DespawnEnemy(unique_id);
        }
        else if (request.Contains("connect") && enemySpawner != null)
        {
            Debug.Log("Network manager handling connect request");
            Debug.Log(request);
            string unique_id = request_params[request_params.Length - 1];
            float x = float.Parse(request_params[1]);
            float y = float.Parse(request_params[2]);
            enemySpawner.CreateEnemy(x, y, unique_id);
        }
        else if (request.Contains("bomb") && bombSpawner != null)
        {
            float x = float.Parse(request_params[1]);
            float y = float.Parse(request_params[2]);
            bombSpawner.SpawnBomb(x, y);
        }
        else if (request.Contains("move") && enemySpawner != null)
        {
            Debug.Log("Network manager handling movement request");
            Debug.Log(request);
            string unique_id = request_params[request_params.Length - 1];

            float x = float.Parse(request_params[1]);
            float y = float.Parse(request_params[2]);

            enemySpawner.MoveEnemy(x, y, unique_id);
        }
    }



    //public async Task HandleRequestAsync(ClientWebSocket socket, string request)
    //{
    //    //seems to be working
    //    Debug.Log(request);
    //    await BroadCastMessageAsync(socket, "i got your request");
    //}

    //public async Task BroadCastMessageAsync(ClientWebSocket socket, string data)
    //{
    //    ArraySegment<byte> myArrSegAll = new ArraySegment<byte>(Encoding.UTF8.GetBytes(data));
    //    await socket.SendAsync(myArrSegAll, WebSocketMessageType.Text, true, CancellationToken.None);

    //}


    //private async Task Send(ClientWebSocket socket, string data)
    //{
    //    ArraySegment<byte> myArrSegAll = new ArraySegment<byte>(Encoding.UTF8.GetBytes(data));
    //    await socket.SendAsync(myArrSegAll, WebSocketMessageType.Text, true, CancellationToken.None);

    //}
}
