using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform movePoint;
    public LayerMask whatStopsMovement;
    public Animator animator;
    public float moveSpeed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        movePoint.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, movePoint.position) <= 0.05f)
        {
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f)
            {
                if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f), 0.2f, whatStopsMovement))
                {
                    movePoint.position += new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
                }
            } else if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f)
            {
                if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f), 0.2f, whatStopsMovement))
                {
                    movePoint.position += new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f);
                }
            }

            animator.SetBool("Moving", false);
        } else
        {
            animator.SetBool("Moving", true);
        }
    }


    //// Start is called before the first frame update
    //public float moveSpeed = 5f;
    //private bool prevStatusHorizontal;
    //private bool prevStatusVertical;
    //private bool moveHorizontal;

    //public Rigidbody2D rb;
    //public Animator animator;

    //Vector2 movement;

    //// Update is called once per frame
    //void Update()
    //{
    //    bool statusHorizontal = Input.GetButton("Horizontal");
    //    bool statusVertical = Input.GetButton("Vertical");

    //    if (statusHorizontal && !prevStatusHorizontal)
    //        moveHorizontal = true;

    //    if (statusVertical && !prevStatusVertical || !statusHorizontal)
    //        moveHorizontal = false;

    //    movement = new Vector2(0, 0);

    //    // Input
    //    if (statusVertical && !moveHorizontal)
    //    {
    //        movement.y = Input.GetAxisRaw("Vertical");
    //    } else if (statusHorizontal)
    //    {
    //        movement.x = Input.GetAxisRaw("Horizontal");
    //    }

    //    animator.SetFloat("Horizontal", movement.x);
    //    animator.SetFloat("Vertical", movement.y);
    //    animator.SetFloat("Speed", movement.sqrMagnitude);

    //    prevStatusHorizontal = statusHorizontal;
    //    prevStatusVertical = statusVertical;
    //}

    //void FixedUpdate()
    //{
    //    // movement
    //    rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    //}

}
