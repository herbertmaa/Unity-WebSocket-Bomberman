using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BombSpawner : MonoBehaviour
{
    public Tilemap tilemap;
    public GameObject bombPrefab;
    public NetworkManager networkManager = null;
    public Queue<Action> jobs = new Queue<Action>();

    void Start()
    {
        networkManager = GameObject.FindWithTag("MainCamera").GetComponent<NetworkManager>();
        if (networkManager == null) Debug.Log("Unable to get a reference of the Network Manager");
    }

    // Update is called once per frame
    void Update()
    {
        while (jobs.Count > 0)
            jobs.Dequeue().Invoke();

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cell = tilemap.WorldToCell(pos);
            Vector3 cellCenterPos = tilemap.GetCellCenterWorld(cell);
            Vector2 bombPosition = cellCenterPos;

            Instantiate(bombPrefab, cellCenterPos, Quaternion.identity);
            BroadCastPlacement(bombPosition.x, bombPosition.y);
        }
    }

    private void BroadCastPlacement(float x, float y)
    {
        Debug.Log("player is sending a bomb message: " + x + " " + y);
        string message = "bomb " + x + " " + y;
        networkManager.BroadCastMessage(message);

    }

    public void SpawnBomb(float x, float y)
    {
        Vector3 position = new Vector3(x, y);
        jobs.Enqueue(() => {
            Instantiate(bombPrefab, position, Quaternion.identity);
        });

    }
}
