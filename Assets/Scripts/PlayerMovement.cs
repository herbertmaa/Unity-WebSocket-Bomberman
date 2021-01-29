using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform movePoint;
    public LayerMask whatStopsMovement;
    public Animator animator;
    public GameObject player;
    public BombSpawner bombSpawner = null;

    private float cooldownCounter = 0;
    public float bombCooldown = 1f;

    public float moveSpeed = 5f;
    public NetworkManager networkManager = null;
    // Start is called before the first frame update


    void Awake()
    {
        movePoint.transform.position = new Vector3(0, 0.25f);
        movePoint.parent = null;
    }
    void Start()
    {
        networkManager = GameObject.FindWithTag("NetworkManager").GetComponent<NetworkManager>();
        bombSpawner = GameObject.FindWithTag("BombSpawner").GetComponent<BombSpawner>();
        if (networkManager == null) Debug.Log("Unable to get a reference of the Network Manager");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        cooldownCounter -= Time.deltaTime;
        
        if (Input.GetKeyUp(KeyCode.Z) || Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log(player.transform.position.x + " " + player.transform.position.y);
            placeBomb();
        }


        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, movePoint.position) <= 0.05f)
        {
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f)
            {
                // Keyleft / KeyRight was pressed
                if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f), 0.2f, whatStopsMovement))
                {
                    checkLeftOrRight();
                    movePoint.position += new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
                    Vector2 v2 = movePoint.position;
                    BroadcastMovement(v2.x, v2.y);
                }
            } else if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f)
            {
                // Keydown / Keyup was pressed
                if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f), 0.2f, whatStopsMovement))
                {
                    movePoint.position += new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f);
                    Vector2 v2 = movePoint.position;
                    BroadcastMovement(v2.x, v2.y);
                }
            }
        }
    }

    private void BroadcastMovement(float x, float y)
    {
        string message = "move " + x + " " + y;
        Debug.Log("player is sending a message: " + message);

        networkManager.BroadCastMessage(message);
    }

    void placeBomb()
    {
        if (cooldownCounter > 0)
        {
            return;
        }

        bombSpawner.SpawnBomb(player.transform.position.x, player.transform.position.y);
        bombSpawner.BroadCastPlacement(player.transform.position.x, player.transform.position.y);
        cooldownCounter = bombCooldown;
    }

    // For Animations
    private void checkLeftOrRight()
    {
        if (Input.GetAxisRaw("Horizontal") == 1)
        {
            animator.SetBool("GoingRight", true);
        } else if (Input.GetAxisRaw("Horizontal") == -1)
        {
            animator.SetBool("GoingRight", false);
        }
    }

}
