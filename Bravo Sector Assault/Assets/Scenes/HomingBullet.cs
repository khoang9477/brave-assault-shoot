using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingBullet : MonoBehaviour
{
    //bullet behavior
    public float homingSpeed = 0.01f;
    public float rotationSpeed = 350f;
    private Transform target;
    private float lifeTime = 15f;

    private void Start() //just in case no player touches homing bullet
    {
        Destroy(gameObject, lifeTime);
    }

    //Locate player
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target == null)
        {
            Debug.Log("Target is located NULL!");
        }
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        //Move toward player
        Vector2 direction = (target.position - transform.position).normalized;
        float rotateAmount = Vector3.Cross(direction, transform.up).z;
        transform.Rotate(0, 0, -rotateAmount * rotationSpeed * Time.deltaTime);
        transform.position += transform.up * homingSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //collision.GetComponent<PlayerController>().TakeDamage(20); // Deal damage
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(20, true);
            }
            Destroy(gameObject);
        }
    }
}
