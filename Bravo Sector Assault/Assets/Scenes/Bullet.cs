using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Control bullet
    public float bulletSpeed = 10f;
    public float bulletLifetime = 3f;

    // Called when bullet reached lifetime to prevent memory issues
    private void Start()
    {
        Destroy(gameObject, bulletLifetime);
    }

    // Update the bullet movement
    private void Update()
    {
        //Moving forward from your ship that is shooting
        transform.Translate(Vector2.up * bulletSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Destroy bullet when hit player
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(5, false); //default damage
            }
            Destroy(gameObject);
        }
        //Destroy bullet when hit enemy
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(5); //default damage
            }
            Destroy(gameObject);
        }
    }
}
