using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : MonoBehaviour
{
    public Collider2D arms1;
    public Collider2D arms2;
    public Collider2D arms3;
    Rigidbody2D rb;

    public float speed;
    public bool active;

    private void Update()
    {
        if (active)
        {
            transform.Rotate (0,0,speed*Time.deltaTime);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Look for player tag, death mechanic
    }
}
