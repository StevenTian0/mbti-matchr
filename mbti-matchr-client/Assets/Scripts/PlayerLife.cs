using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLife : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    [SerializeField] private AudioSource deathSoundEffect;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }


    void OnTriggerEnter2D(Collider2D other){  // The player enters a trigger collider (trap or spikes)
        if (other.gameObject.tag == "Trap")
        {
            Debug.Log("trap!");
            Die();
        }
    }

        private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Trap"))
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("die");
        //deathSoundEffect.Play();
        rb.bodyType = RigidbodyType2D.Static;
        //anim.SetTrigger("death");
        RestartLevel();
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
