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
    public float giantDuration = 25f; // 유지 시간(초)
    private Coroutine giantCoroutine; // 코루틴 중복 방지


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
                pAni.SetBool("isRunning", false);
                break;

            case < 0:
                transform.localScale = isGiant ? new Vector3(-2f, 2f, 1f) : new Vector3(-1f, 1f, 1f);
                pAni.SetBool("isRunning", true);
                break;

            default:
                pAni.SetBool("isRunning", false);
                break;
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        pAni.SetBool("isGrounded", isGrounded);
        pAni.SetFloat("Speed", Mathf.Abs(moveInput));

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            pAni.SetBool("JumpAction", true);
        }

        if (isGrounded && rb.velocity.y <= 0.01f)
        {
            pAni.SetBool("JumpAction", false);
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
                if (isGiant)
                {
                    Destroy(collision.gameObject);
                    Debug.Log("무적에 의한 함정 파괴");
                    jumpForce += 5;
                }
                else
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
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
                if (giantCoroutine != null)
                {
                    StopCoroutine(giantCoroutine); // 기존 코루틴 중단
                }
                jumpForce += 5;
                giantCoroutine = StartCoroutine(GiantMode());
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

    private IEnumerator GiantMode()
    {
        isGiant = true;

        yield return new WaitForSeconds(giantDuration);

        isGiant = false;
    }

}