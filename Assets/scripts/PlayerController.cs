using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator pAni;
    private bool isGrounded;

    private bool isGiant = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pAni = GetComponent<Animator>();
    }

    private void Update()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        switch (moveInput)
        {
            case > 0:
                transform.localScale = isGiant ? new Vector3(2f, 2f, 1f) : new Vector3(1f, 1f, 1f);
                break;

            case < 0:
                transform.localScale = isGiant ? new Vector3(-2f, 2f, 1f) : new Vector3(-1f, 1f, 1f);
                break;
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            pAni.SetTrigger("JumpAction");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Enemy":
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                break;
            case "Trap":

                break;
            case "Respawn":
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                break;
            case "Finish":
                LevelObject levelObject = collision.GetComponent<LevelObject>();
               if (levelObject != null)
                {
                    levelObject.MoveToNextLevel();
                }
                break;
            case "shield":
                isGiant = true;
                Destroy(collision.gameObject);
                break;
            case "Jump":
                jumpForce += 2f;
                Destroy(collision.gameObject);
                break;
            case "Speed":
                moveSpeed += 5f;
                Destroy(collision.gameObject);
                break;
        }
    }
}