using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;

public class MultiCastNetworkManager : NetworkManager
{

    private IPAddress mcastAddress = IPAddress.Parse("230.0.0.1");
    private int mcastPort = 11000;
    private Socket mcastSocket;
    private IPEndPoint endPoint;

    void Awake()
    {
        endPoint = new IPEndPoint(mcastAddress, mcastPort);
    }

    void Update()
    {
        
    }

    void OnDestroy()
    {
        Debug.Log("OnDestroy");
        mcastSocket.SendTo(ASCIIEncoding.ASCII.GetBytes("disconnect"), endPoint);
        Debug.Log("No Errors Thrown?");
        mcastSocket.Close();
    }

    public override void Connect()
    {
        try
        {
            mcastSocket = new Socket(AddressFamily.InterNetwork,
                           SocketType.Dgram,
                           ProtocolType.Udp);

            IPAddress localIPAddr = IPAddress.Parse("0.0.0.0");


            // Create an IPEndPoint object.
            IPEndPoint IPlocal = new IPEndPoint(localIPAddr, 0);

            // Bind this endpoint to the multicast socket.
            mcastSocket.Bind(IPlocal);

            // Define a MulticastOption object specifying the multicast group
            // address and the local IP address.
            // The multicast group address is the same as the address used by the listener.

            MulticastOption mcastOption;
            mcastOption = new MulticastOption(mcastAddress, localIPAddr);

            mcastSocket.SetSocketOption(SocketOptionLevel.IP,
                                        SocketOptionName.AddMembership,
                                        mcastOption);

            //Send multicast packets to the listener.
            mcastSocket.SendTo(ASCIIEncoding.ASCII.GetBytes("new_connection"), endPoint);

            Debug.Log("connection to server established");


            receiveThread = new Thread(new ThreadStart(ReceiveData));
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log("Failed to connect to server");
        }
    }

    public void ReceiveData()
    {

        if (mcastSocket == null) return;

        EndPoint remoteEP = (EndPoint)new IPEndPoint(mcastAddress, 0);
        byte[] bytes = new Byte[100];
        bool connected = true;

        try
        {
            while (connected)
            {
                mcastSocket.ReceiveFrom(bytes, ref remoteEP);
                String msg = Encoding.ASCII.GetString(bytes, 0, bytes.Length);

                // Clear the buffer
                Array.Clear(bytes, 0, bytes.Length);

                Thread handleThread = new Thread(() => HandleRequest(msg));
                Debug.Log("after execution");
                handleThread.IsBackground = true;
                handleThread.Start();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log("Connection interrupt");
        }
    }

    public override void HandleRequest(String request)
    {
        request = request.Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
        request = Regex.Replace(request, @"[^\u0009\u000A\u000D\u0020-\u007E]", "");

        string[] request_params = request.Split(' ');
        
        //Handling messages from server.
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

    public override void BroadCastMessage(String message)
    {
        mcastSocket.SendTo(ASCIIEncoding.ASCII.GetBytes(message), endPoint);

    }
}
