using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinnishZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Player>())
        {
            EventBus.Instance.levelFinnished.Invoke();
        }
    }
}
