using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType { Invincibility, Heal, SpeedBoost, SlowEnemy }
    public PowerUpType powerUpType;
    public float duration = 3f; //for now 3 seconds duration. If possible, put a random range
    public int healPlayer = 20; //heal 20 hp
    public float despawnTimer = 15f;

    //get an audio for player collect
    public AudioClip collectPowerUpSound;
    private AudioSource audioSource;


    private void Start()
    {
        //Link audio
        audioSource = GetComponent<AudioSource>();

        Destroy(gameObject, despawnTimer);
    }

    private void OnTriggerEnter2D(Collider2D collide)
    {
        if (collide.CompareTag("Player"))
        {
            PlayerController player = collide.GetComponent<PlayerController>();

            if (player != null)
            {
                AddPowerUp(player);

                player.CollectPowerUp();
            }

            Destroy(gameObject); //remove when player collect
        }
    }


    private void AddPowerUp(PlayerController player)
    {
        switch (powerUpType)
        {
            case PowerUpType.Invincibility:
                player.StartCoroutine(player.Invincible(duration));
                break;
            case PowerUpType.Heal:
                player.Heal(healPlayer);
                break;
            case PowerUpType.SpeedBoost:
                player.StartCoroutine(player.SpeedBoost(duration));
                break;
            case PowerUpType.SlowEnemy:
                player.StartCoroutine(player.SlowEnemy(duration));
                break;
        }
    }
}
