
using UnityEngine;
using System;
using System.Threading;

public abstract class NetworkManager : MonoBehaviour
{
    protected Thread receiveThread;
    public BombSpawner bombSpawner = null;
    public EnemySpawner enemySpawner = null;
    public PlayerSpawner playerSpawner = null;

    void Start()
    {
        bombSpawner = GameObject.FindWithTag("BombSpawner").GetComponent<BombSpawner>();
        enemySpawner = GameObject.FindWithTag("EnemySpawner").GetComponent<EnemySpawner>();
        playerSpawner = GameObject.FindWithTag("PlayerSpawner").GetComponent<PlayerSpawner>();

        if (bombSpawner == null) Debug.Log("Unable to get a reference of the Bomb Spawner");
        if (enemySpawner == null) Debug.Log("Unable to get a reference of the Enemy Spawner");
        if (playerSpawner == null) Debug.Log("Unable to get a reference of the Player Spawner");

        Debug.Log("network manager");
        Connect();
    }

    public abstract void Connect();

    public abstract void HandleRequest(String request);

    public abstract void BroadCastMessage(String message);

}