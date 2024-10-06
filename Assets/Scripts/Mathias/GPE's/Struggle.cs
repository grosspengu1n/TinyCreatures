using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Struggle : MonoBehaviour
{
    public bool vulnerable;
    public bool caught;
    public GameObject currentTrap;

    public int mashCounter;
    private void Update()
    {
        if (caught) 
        {
            //turn off player movement
            //swap sprite for hypnotized mosquito
            //pullplayer to killbox
            //count struggles with get button down both leftmouse and controller?
            //if enough mashes, turn caught off, set vulnerable to false 
            //start corroutine in 5sec
        }
    }

    //make coroutine that sets vulnerable to true after 5 sec
}
