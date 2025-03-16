using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //for UI elements

public class PlayerController : MonoBehaviour
{
    //Screen boundaries
    private Vector2 screenBounds;
    private float playerWidth;
    private float playerHeight;

    //Control player
    public float moveSpeed = 3f;
    public bool isInvincible = false; //only false to prevent immune
    public string horizontalInput;
    public string verticalInput;
    public bool isPlayer1 = true; //flag if player 1 is controlled?
    public GameObject bulletPrefab; //reference to bullet's player
    public Transform fireBullet; //bullet spawn

    //player score
    public int score = 0;
    public UIManager uiManager;

    //control player health
    public int maxHealth = 100;
    public int currentHealth;
    public Slider healthBar; //for display player health bar

    //audio source
    public AudioSource audioSource;
    public AudioClip playerShootSound;
    public AudioClip powerUpSound;
    public AudioClip regularDamageSound;
    public AudioClip homingDamageSound;
    public AudioClip playerDestroyedSound;
    public AudioClip powerUpExpireSound;
    public AudioClip playerRespawnSound;

    //first frame for load player health, calculate screen limits, load the sound
    private void Start()
    {
        //Link Audio
        audioSource = GetComponent<AudioSource>();

        //Player Health
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;

        screenBounds = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.transform.position.z));

        //Get player size
        playerWidth = GetComponent<SpriteRenderer>().bounds.extents.x;
        playerHeight = GetComponent<SpriteRenderer>().bounds.extents.y;
    }

    // update for player input each frame and including restrict from offscreen
    private void Update()
    {
        //checking condition for each player
        float moveX = Input.GetAxis(horizontalInput) * moveSpeed * Time.deltaTime;
        float moveY = Input.GetAxis(verticalInput) * moveSpeed * Time.deltaTime;

        Vector3 newPosition = transform.position + new Vector3(moveX, moveY, 0); //calculate new screen psotion

        //Screen bounds
        newPosition.x = Mathf.Clamp(newPosition.x, -screenBounds.x + playerWidth, screenBounds.x - playerWidth);
        newPosition.y = Mathf.Clamp(newPosition.y, -screenBounds.y + playerHeight, screenBounds.y - playerHeight);

        transform.position = newPosition; //set the screen bounds

        //player input
        Shoot();
        Move();
    }

    //update for player movement
    private void Move()
    {
        float horizontal = isPlayer1 ? Input.GetAxis("Horizontal") : Input.GetAxis("Horizontal2");
        float vertical = isPlayer1 ? Input.GetAxis("Vertical") : Input.GetAxis("Vertical2"); ;

        Vector2 playerMove = new Vector2(horizontal, vertical);
        transform.Translate(playerMove * moveSpeed * Time.deltaTime);
    }

    //update for player shoots bullet
    private void Shoot()
    {
        if (isPlayer1 && Input.GetKeyDown(KeyCode.Space))
        {
            ShootBullet();
        }
        else if (!isPlayer1 && Input.GetKeyDown(KeyCode.RightControl))
        {
            ShootBullet();
        }
    }

    //spawn bullet to shoot
    private void ShootBullet()
    {
        audioSource.PlayOneShot(playerShootSound);
        Instantiate(bulletPrefab, fireBullet.position, fireBullet.rotation);
    }

    //when player taking damage
    public void TakeDamage(int damage, bool isHoming)
    {
        if (isInvincible)
        {
            return;
        }   

        currentHealth -= damage;
        //Play sound based on bullet type
        if (isHoming)
        {
            audioSource.PlayOneShot(homingDamageSound);
        }
        else
        {
            audioSource.PlayOneShot(regularDamageSound);
        }

        healthBar.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    //when player loses
    private void Die()
    {
        // Find global audio source
        AudioSource globalAudioSource = GameObject.Find("AudioDeathManager").GetComponent<AudioSource>();

        if (globalAudioSource != null && playerDestroyedSound != null)
        {
            globalAudioSource.PlayOneShot(playerDestroyedSound);
        }
        Debug.Log(gameObject.name + " has been destroyed!");
        gameObject.SetActive(false); //disable dead player


        //respawn mechanic
        Invoke(nameof(Respawn), 5f); //respawn player 5 seconds
    }

    private void Respawn()
    {
        currentHealth = maxHealth;
        healthBar.value = currentHealth;

        //respawn in random location
        transform.position = new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, 3f));
        Debug.Log(gameObject.name + " respawned!");

        //dead player now alive
        gameObject.SetActive(true);

        audioSource.PlayOneShot(playerRespawnSound);
    }

    //Power Up Method each case//
    //Invincible
    public IEnumerator Invincible(float time)
    {
        isInvincible = true; //set this to player immune to deal damage
        Debug.Log(gameObject.name + " invincible!");

        yield return new WaitForSeconds(time);
        audioSource.PlayOneShot(powerUpExpireSound);
        isInvincible = false; //set this to player no longer invincibility
        Debug.Log(gameObject.name + " vulnerable!");
    }

    //Heal
    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        healthBar.value = currentHealth;
        Debug.Log(gameObject.name + " heals!");
    }

    //Speed Boost
    public IEnumerator SpeedBoost(float time)
    {
        moveSpeed *= 1.5f; //move faster
        Debug.Log(gameObject.name + " speeds up!");
        yield return new WaitForSeconds(time);
        audioSource.PlayOneShot(powerUpExpireSound);
        moveSpeed /= 1.5f; //move slower (reset)
        Debug.Log(gameObject.name + " speeds reset!");
    }

    //Slow Down
    public IEnumerator SlowEnemy(float time)
    {
        PlayerController[] players = FindObjectsOfType<PlayerController>(); //reference to players
        foreach (PlayerController p in players) //locate all players
        {
            //only opponent slow down
            if (p != this)
            {
                Debug.Log("Opponent slows down!");
                p.moveSpeed /= 1.5f;
            }
        }
        yield return new WaitForSeconds(time);
        audioSource.PlayOneShot(powerUpExpireSound);
        foreach (PlayerController p in players)
        {
            //only opponent speed up (reset)
            if (p != this)
            {
                Debug.Log("Opponent speeds reset!");
                p.moveSpeed *= 1.5f;
            }
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        if (uiManager != null)
        {

        }
    }

    public void CollectPowerUp()
    {
        audioSource.PlayOneShot(powerUpSound);
    }
}
