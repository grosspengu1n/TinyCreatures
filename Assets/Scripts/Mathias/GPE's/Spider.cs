using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : MonoBehaviour
{
    public Collider2D col;
    public GameObject spiderBody;
    public GameObject spiderShadow;
    public float speed;
    public float pause;

    private float scalingFramesLeft;
    public bool goingIn;
    public bool goingOut;

    private Color shadowColor;
    private void Awake()
    {
        //Enter();
        //shadowColor = shadow.color;
        //shadowColor.a = 0;
    }

    private void Update()
    {
        
    }
    /*public void Enter()
    {
        goingIn = true;
        if (goingIn)
        {
            if (scalingFramesLeft > 0)
            {
                transform.position += new Vector3(0,-speed,0);
                transform.localScale = Vector3.Lerp(transform.localScale, transform.localScale * 2, Time.deltaTime * 10);
                scalingFramesLeft--;
            }
            shadowColor.a += Time.deltaTime;
            if (shadowColor.a >= 1f && scalingFramesLeft <= 0)
            {
                col.enabled = true;
                goingIn = false;
            }
        }
        if (!goingIn)
        {
            if (scalingFramesLeft <= 0)
            {
                scalingFramesLeft++;
                if (scalingFramesLeft >= pause)
                {
                    Exit();
                }
            }
        }
    }
    public void Exit()
    {
        goingOut = true;
        scalingFramesLeft = 10;
        if (goingOut)
        {
            if (scalingFramesLeft <= 10)
            {
                transform.position += new Vector3(0, +speed, 0);
                transform.localScale = Vector3.Lerp(transform.localScale, transform.localScale * -2, Time.deltaTime * 10);
                scalingFramesLeft++;
            }
            col.enabled = false;
            shadowColor.a -= Time.deltaTime;
            if (shadowColor.a <= 0f)
            {
                goingOut = false;
                Destroy(this);
            }
        }
    }*/
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Look for player tag, death mechanic
    }
}
