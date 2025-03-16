using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegularBullet : MonoBehaviour
{
    //bullet behavior
    public float bulletSpeed = 1f;
    private Vector2 direction;
    private float lifeTime = 10f;

    private void Start() //just in case no player touches regular bullet
    {
        Destroy(gameObject, lifeTime);
    }

    public void SetTarget (Transform target)
    {
        if (target != null)
        {
            direction = (target.position - transform.position).normalized;
        }
        else
        {
            direction = Vector2.up; //default if no target found
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Move to the position
        transform.position = Vector2.MoveTowards(transform.position, (Vector2)transform.position + direction, bulletSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Bullet hit: " + collision.gameObject.name);

        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(5, false);
            }
            Destroy(gameObject);
        }
    }
}
