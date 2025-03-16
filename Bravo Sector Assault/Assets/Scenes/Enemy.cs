using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //enemy behavior
    public float moveSpeed = 1.5f;
    public int maxHealth = 100;
    private int currentHealth;
    private Vector2 moveDirection;
    private bool retreat = false;

    //point for enemy destroyed
    public int point = 1;
    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        StartCoroutine(ChangeDirection());
        StartCoroutine(RetreatAfter(30f));
    }

    // Update is called once per frame
    void Update()
    {
        if (retreat)
        {
            moveSpeed = 3f;
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;
        }
        else
        {
            //Random direction
            transform.position += (Vector3)moveDirection * moveSpeed * Time.deltaTime;
            CheckBounds(); //prevent offscreen
        }
    }

    private IEnumerator ChangeDirection()
    {
        while(!retreat)
        {
            moveDirection = Random.insideUnitCircle.normalized;
            yield return new WaitForSeconds(Random.Range(3f, 5f));
        }
    }

    private IEnumerator RetreatAfter(float time)
    {
        yield return new WaitForSeconds(time);
        retreat = true;
        yield return new WaitForSeconds(7.5f);
        Destroy(gameObject); //Remove Enemy
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Destroy(gameObject); //enemy killed
        }
    }

    private void CheckBounds()
    {   //Each reverse direction
        Vector3 position = Camera.main.WorldToViewportPoint(transform.position);
        if (position.x <= 0f || position.x >= 1f)
        {
            moveDirection.x *= -1;
        }
        if (position.y <= 0f || position.y >= 1f)
        {
            moveDirection.y *= -1;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().TakeDamage(50, true);
            Destroy(gameObject); //when player touch enemy, it destroyed
        }
    }

    private void OnDestroy()
    {
        if (playerController != null)
        {
            playerController.AddScore(point);
        }
    }
}
