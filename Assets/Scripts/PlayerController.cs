using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb;
    private GameObject focalPoint;
    private float powerupStrength = 15.0f;
    private int powerUpDuration = 7;

    public float speed = 5.0f;
    public bool hasPowerUp = false;
    public GameObject powerupIndicator;

    // Homing rocket stuff
    public PowerUpType currentPowerUp = PowerUpType.None;
    public GameObject rocketPrefab;
    private GameObject tmpRocket;
    private Coroutine powerupCountdown;

    // Smash power
    private float hangTime = 0.2f;
    private float smashSpeed = 50.0f;
    private float explosionForce = 50.0f;
    private float explosionRadius = 20.0f;
    private bool smashing = false;
    private float floorY;

    // Start is called before the first frame update
    void Start()
    {
        // Get player rigidbody
        playerRb = GetComponent<Rigidbody>();
        // Reach the focalpoint game object
        focalPoint = GameObject.Find("Focal Point");
    }

    // Update is called once per frame
    void Update()
    {
        // Get vertical axis
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 powerupOffset = new Vector3(0, -0.5f, 0);

        // Move the player forward or backwards based on input and focal point(camera) rotation
        playerRb.AddForce(focalPoint.transform.forward * verticalInput * speed);

        // Teleport the powerupIndicator under the player
        powerupIndicator.transform.position = transform.position + powerupOffset;

        // If current poweruptype is rocket and the player presses F
        if (currentPowerUp == PowerUpType.Rockets && Input.GetKeyDown(KeyCode.F))
        {
            // Launch
            LaunchRockets();
        }
        // If current poweruptype is smash and the player presses F
        if (currentPowerUp == PowerUpType.Smash && Input.GetKeyDown(KeyCode.Space) && !smashing)
        {
            smashing = true;
            StartCoroutine(Smash());
        }

        // When you fall you lose
        if (transform.position.y < -10)
        {
            Debug.Log("Game Over!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // When you pick a powerup
        if (other.CompareTag("Powerup"))
        {
            hasPowerUp = true;
            // Set the currentpoweup type to the picked powerups type
            currentPowerUp = other.gameObject.GetComponent<PowerUp>().powerUpType;
            // Activate the powerupIndicator object so it is visible now
            powerupIndicator.SetActive(true);
            // Destroy powerup object
            Destroy(other.gameObject);
            if (powerupCountdown != null)
            {
                // If player already has a powerup stop the countdown
                StopCoroutine(powerupCountdown);
            }
            // Start the countdown
            powerupCountdown = StartCoroutine(PowerupCountdownRoutine());
        }
    }

    IEnumerator PowerupCountdownRoutine()
    {
        // Wait for 7 seconds
        yield return new WaitForSeconds(powerUpDuration);
        // Reset the powerup state of the player
        hasPowerUp = false;
        currentPowerUp = PowerUpType.None;
        // Deactivate the powerupIndicator object so it is invisible now
        powerupIndicator.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If the taken powerup is a Pushback type
        if (collision.gameObject.CompareTag("Enemy") && currentPowerUp == PowerUpType.Pushback)
        {
            // Get enemy rigidbody so we can apply force on it from this script
            Rigidbody enemyRb = collision.gameObject.GetComponent<Rigidbody>();
            // FÝnd the vector away from the player by subtracting the enemies position from plaers position so we can push the enemy to that direction once they collide
            Vector3 awayFromPlayer = collision.gameObject.transform.position - transform.position;

            // Add force to the enemy
            enemyRb.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse);
            // Debug
            Debug.Log("Player collided with: " + collision.gameObject.name + " with powerup set to " + currentPowerUp.ToString());
        }

        if (collision.gameObject.CompareTag("Strong Enemy"))
        {
            // Get enemy rigidbody so we can apply force on it from this script
            Rigidbody enemyRb = collision.gameObject.GetComponent<Rigidbody>();
            // FÝnd the vector away from the player by subtracting the enemies position from plaers position so we can push the enemy to that direction once they collide
            Vector3 awayFromEnemy = transform.position - collision.gameObject.transform.position;

            // Add force to the enemy
            enemyRb.AddForce(awayFromEnemy * powerupStrength / 1.2f, ForceMode.Impulse);
        }
    }

    private void LaunchRockets()
    {
        // For each game object that has Enemy script
        foreach (var enemy in FindObjectsOfType<Enemy>())
        {
            // Create a rocket prefab and assign it to a gameobject variable
            tmpRocket = Instantiate(rocketPrefab, transform.position + Vector3.up, Quaternion.identity);
            // Run the Fire method in that created rocket game object
            tmpRocket.GetComponent<RocketBehaviour>().Fire(enemy.transform);
        }
    }

    IEnumerator Smash()
    {
        var enemies = FindObjectsOfType<Enemy>();
        //Store the y position before taking off
        floorY = transform.position.y;
        //Calculate the amount of time we will go up
        float jumpTime = Time.time + hangTime;
        while (Time.time < jumpTime)
        {
            //move the player up while still keeping their x velocity.
            playerRb.velocity = new Vector2(playerRb.velocity.x, smashSpeed);
            yield return null;
        }
        //Now move the player down
        while (transform.position.y > floorY)
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, -smashSpeed * 2);
            yield return null;
        }
        //Cycle through all enemies.
        for (int i = 0; i < enemies.Length; i++)
        {
            //Apply an explosion force that originates from our position.
            if (enemies[i] != null)
            {
                enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius, 0.0f, ForceMode.Impulse);
            }     
        }
        //We are no longer smashing, so set the boolean to false
        smashing = false;
    }
}
