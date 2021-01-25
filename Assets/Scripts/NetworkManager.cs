
using UnityEngine;
using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

public class NetworkManager : MonoBehaviour
{
    public Thread receiveThread;
    static IPAddress mcastAddress = IPAddress.Parse("230.0.0.1");
    static int mcastPort = 11000;
    public Socket mcastSocket;
    public IPEndPoint endPoint = new IPEndPoint(mcastAddress, mcastPort);
    public BombSpawner bombSpawner = null;

    // Start is called before the first frame update
    void Start()
    {
        Connect();
        bombSpawner = GameObject.FindWithTag("BombSpawner").GetComponent<BombSpawner>();
        if (bombSpawner == null) Debug.Log("Unable to get a reference of the Bomb Spawner");

    }

    // Update is called once per frame
    void Update()
    {

    }

    void Connect()
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
            mcastSocket.SendTo(ASCIIEncoding.ASCII.GetBytes("connect 0 0"), endPoint);

            Debug.Log("connection to server established");


            receiveThread = new Thread(new ThreadStart(ReceiveData));
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log("failed to connect to server");
        }
    }


    void OnDestroy()
    {
        Debug.Log("OnDestroy");
        mcastSocket.SendTo(ASCIIEncoding.ASCII.GetBytes("disconnect"), endPoint);
        Debug.Log("No Errors Thrown?");
        mcastSocket.Close();
    }


    private void ReceiveData()
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
                Debug.Log(msg);

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

    private void HandleRequest(String request)
    {
        Debug.Log("Network manager handling server request");

        string[] request_params = request.Split(' ');

        if (request.Contains("bomb") && bombSpawner != null)
        {
            float x = float.Parse(request_params[1]);
            float y = float.Parse(request_params[2]);
            bombSpawner.SpawnBomb(x, y);
        }
    }

    public void BroadCastMessage(String message)
    {
        mcastSocket.SendTo(ASCIIEncoding.ASCII.GetBytes(message), endPoint);

    }

}