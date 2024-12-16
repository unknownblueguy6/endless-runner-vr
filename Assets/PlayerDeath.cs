using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("Hit obstacle");
            Die();
        }
    }

    private void Die()
    {
        // Disable only the movement script instead of the whole GameObject
        GetComponent<PlayerTestControls>().enabled = false;
        GetComponent<PlayerController>().enabled = false;

        // Start delayed restart from GameManager instead
        GameManager.Instance.StartGameRestart();
    }
}


