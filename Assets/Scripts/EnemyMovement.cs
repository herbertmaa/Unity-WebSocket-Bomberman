using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;


public class EnemyMovement : MonoBehaviour
{
    public LayerMask whatStopsMovement;
    public Animator animator;
    public float moveSpeed = 5f;
    public float newX;
    public float newY;
    private float currX;
    private float currY;
    public Queue<Action> jobs = new Queue<Action>();
    public GameObject enemyPrefab;
    public Tilemap tilemap;
    public Rigidbody2D body;
    public Transform movePoint;

    void Awake()
    {
        movePoint.parent = null;
        if(movePoint.parent != null) Debug.Log("ERROR: Movepoint of the enemy is not set to null");
    }

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }


    // Update is called once per frame
    void FixedUpdate()
    {

        // Move to the new position
        transform.position = Vector3.MoveTowards(transform.position, movePoint.transform.position, moveSpeed * Time.deltaTime);

        // Check if moving horizontal
        if (Mathf.Abs(newX - currX) == 1f)
        {
            // Check if the new location contains an object on the layer "StopsMovement"
            if (!Physics2D.OverlapCircle(movePoint.transform.position + new Vector3(newX, 0f, 0f), 0.2f, whatStopsMovement))
            {
                checkLeftOrRight(); // this never gets reached
                movePoint.transform.position += new Vector3((newX - currX), 0f, 0f);
                currX = newX;
            }
            // Check if moving vertical
        }
        else if (Mathf.Abs(newY - currY) == 1f)
        {
            // Check if the new space contains an object on the layer "StopsMovement"
            if (!Physics2D.OverlapCircle(movePoint.transform.position + new Vector3(newY, 0f, 0f), 0.2f, whatStopsMovement))
            {
                movePoint.transform.position += new Vector3((newY - currY), 0f, 0f);
                currY = newY;
            }
        }

    }



    private void checkLeftOrRight()
    {
        if ((newX - currX) >= 0.8)
        {
            Debug.Log("before GoingRight = true");
            animator.SetBool("GoingRight", true);
        }
        else if ((newX - currX) <= -0.8)
        {
            Debug.Log("before GoingRight = false");
            animator.SetBool("GoingRight", false);
        }
    }


    public void MoveEnemy(float x, float y)
    {
        newX = x;
        newY = y;

        //Use the two store floats to create a new Vector2 variable movement.
        Vector2 newLocation = new Vector2(newX, newY);
        checkLeftOrRight();
        movePoint.transform.position = newLocation;

        currX = newX;
        currY = newY;
    }
}
