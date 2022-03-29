using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody enemyRb;
    private GameObject player;

    public float speed = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        // Get enemy rigidbody
        enemyRb = GetComponent<Rigidbody>();
        // Reach the player game object
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // Find the vector towards the player by subtracting the players position from enemies position and then normalize it
        Vector3 lookDirection = (player.transform.position - transform.position).normalized;
        // Move the enemy towards the player
        enemyRb.AddForce(lookDirection * speed);

        if (transform.position.y < -10)
        {
            Destroy(gameObject);
        }
    }
}
