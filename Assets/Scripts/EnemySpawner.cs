using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Queue<Action> jobs = new Queue<Action>();
    public GameObject enemyPrefab;
    public Dictionary<string, GameObject> enemies = new Dictionary<string, GameObject>();

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


    public void CreateEnemy(float x, float y, string unique_id)
    {
        Vector3 position = new Vector3(x, y);
        unique_id = unique_id.Trim();

        if (enemies.ContainsKey(unique_id)) return;

        jobs.Enqueue(() => {

            GameObject newEnemy = Instantiate(enemyPrefab, position, Quaternion.identity);
            enemies.Add(unique_id, newEnemy);
        });
    }

    public void MoveEnemy(float x, float y, string unique_id)
    {
        jobs.Enqueue(() => {

            Debug.Log("Trying to move enemy: " + unique_id);
            Debug.Log(enemies.ContainsKey(unique_id));

            if (!enemies.ContainsKey(unique_id)) return;

            jobs.Enqueue(() => {
                GameObject enemy = enemies[unique_id];
                EnemyMovement moveScript = enemy.GetComponent<EnemyMovement>();
                moveScript.MoveEnemy(x, y);
            });


        });
        
    }


    public void DespawnEnemy(string unique_id)
    {
        jobs.Enqueue(() => {

            Debug.Log("Trying to despawn enemy: " + unique_id);
            if (!enemies.ContainsKey(unique_id)) return;

            jobs.Enqueue(() => {
                GameObject enemy = enemies[unique_id];
                enemies.Remove(unique_id);
                Destroy(enemy);
            });


        });
    }

}
