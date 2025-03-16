using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    //enemy shooting behavior
    public GameObject bulletPrefab;
    public GameObject homingBulletPrefab;
    public Transform firePoint;
    public float fireRate = 2f;
    public float homingBulletCooldown = 9.5f;
    public bool isRetreatShip = false;

    //get a bullet sound effect

    public AudioClip enemyRegularBulletShootSound;
    public AudioClip enemyHomingBulletShootSound;
    private AudioSource audioSource;

    private void Start()
    {
        //Link audio
        audioSource = GetComponent<AudioSource>();
        if (!isRetreatShip)
        {
            StartCoroutine(Shoot());
            StartCoroutine(HomingShoot());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();

            if (player != null)
            {
                if (player.currentHealth <= 0)
                {
                    Enemy enemy = GetComponentInParent<Enemy>();
                    if (enemy != null)
                    {
                        PlayerController enemyPlayer = enemy.GetComponent<PlayerController>();
                        if (enemyPlayer != null)
                        {
                            enemyPlayer.AddScore(5);
                        }
                    }
                }
            }
            Destroy(gameObject);
        }
    }

    // Regular Shooting (Every Fire Rate Interval)
    private IEnumerator Shoot()
    {
        while (true)
        {
            if (isRetreatShip)
            {
                yield return null;
                fireRate = 1000f;
            }
            else
            {
                yield return new WaitForSeconds(fireRate);

                PlayerController targetPlayer = FindClosestPlayer();

                if (targetPlayer != null)
                {
                    GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                    RegularBullet regularScript = bullet.GetComponent<RegularBullet>();
                    if (regularScript != null)
                    {
                        audioSource.PlayOneShot(enemyRegularBulletShootSound);
                        fireRate = Random.Range(2f, 3f);
                        regularScript.SetTarget(targetPlayer.transform);
                    }
                    else
                    {
                        Debug.Log("Regular Bullet Script is missing. Instantly removed!");
                    }
                }
                else
                {
                    Debug.Log("No regular bullet can shoot!");
                }
            }
        }
    }

    // Homing Missile Shooting (Every HomingBulletCooldown Interval)
    private IEnumerator HomingShoot()
    {
        while (true)
        {
            if (isRetreatShip)
            {
                yield return null;
                homingBulletCooldown = 1000f;
            }
            else
            {
                yield return new WaitForSeconds(homingBulletCooldown);

                PlayerController leader = FindLeaderPlayer();

                if (leader == null)
                {
                    continue; //no leader found!
                }

                if (homingBulletPrefab == null)
                {
                    Debug.LogError("Your homing bullet is not found! No homing bullet can shoot!");
                    yield break;
                }

                if (firePoint == null)
                {
                    Debug.LogError("Your fire pointer is not found! Enemy ship cannot shoot!");
                    yield break;
                }

                GameObject homingBullet = Instantiate(homingBulletPrefab, firePoint.position, Quaternion.identity);
                HomingBullet homingScript = homingBullet.GetComponent<HomingBullet>();

                if (homingScript != null)
                {
                    audioSource.PlayOneShot(enemyHomingBulletShootSound);
                    homingBulletCooldown = Random.Range(8f, 12.5f);
                    homingScript.SetTarget(leader.transform);
                }
                else
                {
                    Debug.LogError("Homing script is missing, therefore no bullet can shoot!");
                    Destroy(homingBullet);
                }
            }
        }
    }

    // Find the Player with the Highest Health (Leader)
    private PlayerController FindLeaderPlayer()
    {
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        PlayerController leader = null;
        int highestHealth = 0;

        foreach (var player in players)
        {
            if (player.currentHealth >= highestHealth)
            {
                highestHealth = player.currentHealth;
                leader = player;
            }
        }

        return leader;
    }

    //Find Closest Player to Shoot Regular Bullet
    private PlayerController FindClosestPlayer()
    {
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        PlayerController closestPlayer = null;
        float shortestDistance = Mathf.Infinity;

        foreach (var player in players)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closestPlayer = player;
            }
        }

        return closestPlayer;
    }
}
