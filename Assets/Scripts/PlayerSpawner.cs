using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{

    public GameObject playerPrefab;
    private PlayerMovement player = null;
    public Queue<Action> jobs = new Queue<Action>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        while (jobs.Count > 0)
            jobs.Dequeue().Invoke();
    }

    public void SpawnPlayer(float x, float y)
    {
        if (player != null) return;

        Vector3 position = new Vector3(x, y);
        jobs.Enqueue(() => {
            GameObject myplayer = Instantiate(playerPrefab, position, Quaternion.identity);
            myplayer.GetComponent<PlayerMovement>().movePoint.position = position;
            player = myplayer.GetComponent<PlayerMovement>();
        });

    }

    public void DespawnPlayer()
    {
        if (player != null) {
            Destroy(player.gameObject);
            GameObject.FindWithTag("NetworkManager").GetComponent<NetworkManager>().BroadCastMessage("disconnect");
            Application.Quit();
        }
           
    }
}
