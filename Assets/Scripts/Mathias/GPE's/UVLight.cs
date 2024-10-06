using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVLight : MonoBehaviour
{
    public Collider2D pullCol;

    public GameObject killbox;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(""))
        {
            if (collision.gameObject.GetComponent<Struggle>().vulnerable)
            {
                collision.gameObject.GetComponent<Struggle>().currentTrap = killbox;
                collision.gameObject.GetComponent<Struggle>().caught = true;
            }

        }


    }
}
