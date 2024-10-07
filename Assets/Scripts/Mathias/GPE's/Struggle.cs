using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Struggle : MonoBehaviour
{
    public bool vulnerable;
    public bool caught;
    public GameObject currentTrap;
    public int currentTrapStrength;
    public float pullSpeed;
    //Modify these witht he permanent modifiers
    public int mashCounter;
    private void Update()
    {
        if (caught) 
        {
            if (currentTrap.CompareTag ("UVLight"))
            {
                //turn off player movement
                this.GetComponent<Movement>().state = "hypno";
                //swap sprite for hypnotized mosquito

                //pullplayer to killbox
                transform.position = Vector3.Lerp(transform.position, currentTrap.transform.position, pullSpeed);
                //count struggles with get button down both leftmouse and controller?
                if (Input.anyKey)
                {
                    //add rumble on each mash
                    mashCounter++;
                    //add an extra ++ if they have the upgrade
                }
                

            }

        }
        if (mashCounter >= currentTrapStrength)
        {
            //if enough mashes, turn caught off, set vulnerable to false , set mash counter to zeor
            vulnerable = false;
            caught = false;
            //start vulnerable corroutine in 5sec

            this.GetComponent<Movement>().state = "free";
            mashCounter = 0;
        }
    }

    //make coroutine that sets vulnerable to true after 5 sec
}
