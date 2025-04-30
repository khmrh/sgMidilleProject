using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
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
    public float giantDuration = 25f; // ���� �ð�(��)
    private Coroutine giantCoroutine; // �ڷ�ƾ �ߺ� ����
    private float originalMoveSpeed;
    private float originalJumpForce;

    public BuffUIManager uiManager; // �ν����� ����

    private enum SizeState { Normal, Mini, Giant }
    private SizeState currentSizeState = SizeState.Normal;


    private bool isCmini = false;
    private Coroutine sizeBuffCoroutine;

    float score;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pAni = GetComponent<Animator>();

        originalMoveSpeed = moveSpeed;
        originalJumpForce = jumpForce;

        score = 1000f;
    }

    private void Update()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        switch (moveInput)
        {
            case > 0:
                ApplyScale(true);  // ������
                pAni.SetBool("isRunning", true);
                break;

            case < 0:
                ApplyScale(false); // ����
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

        score -= Time.deltaTime;
    }

    private void ApplyScale(bool facingRight)
    {
        float x = 1f, y = 1f;

        switch (currentSizeState)
        {
            case SizeState.Giant:
                x = 2f;
                y = 2f;
                break;
            case SizeState.Mini:
                x = 0.5f;
                y = 0.5f;
                break;
            case SizeState.Normal:
                x = 1f;
                y = 1f;
                break;
        }

        // ���� ���� (�����̸� x ��ȣ ����)
        transform.localScale = new Vector3(facingRight ? x : -x, y, 1f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Enemy":
                if (isGiant)
                {
                    Destroy(collision.gameObject);
                    Debug.Log("������ ���� �� �ı�");
                    StartCoroutine(TemporaryBoost("speed", 5f, 0f, 10f)); // ����: 10�ʰ� �ӵ� ����
                }
                else
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
                break;
            case "Trap":
                if (isGiant)
                {
                    Destroy(collision.gameObject);
                    Debug.Log("������ ���� ���� �ı�");
                    StartCoroutine(TemporaryBoost("jump", 0f, 5f, 10f)); // ����: 10�ʰ� ������ ����
                }
                else
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
                break;
            case "Respawn":
                if (isGiant)
                {
                   
                    Debug.Log("�������� ���� ����");
                }
                else
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
                break;
            case "Finish":

                HighScore.TrySet(SceneManager.GetActiveScene().buildIndex, (int)score);

                LevelObject levelObject = collision.GetComponent<LevelObject>();
                if (levelObject != null)
                {
                    levelObject.MoveToNextLevel();
                }
                break;
            case "shield":
                if (giantCoroutine != null) StopCoroutine(giantCoroutine);
                giantCoroutine = StartCoroutine(GiantMode(10f)); // 10�� ���� ����
                uiManager.ShowBuff("giant", 10f);
                Destroy(collision.gameObject);
                break;
            case "Jump":
                StartCoroutine(TemporaryBoost("jump", 0f, 5f, 10f));  // Jump ����
                uiManager.ShowBuff("jump", 10f);
                Destroy(collision.gameObject);
                break;
            case "Speed":
                StartCoroutine(TemporaryBoost("speed", 5f, 0f, 10f));  // Speed ����
                uiManager.ShowBuff("speed", 15f);
                if (sizeBuffCoroutine != null) StopCoroutine(sizeBuffCoroutine);
                sizeBuffCoroutine = StartCoroutine(MiniMode(15f)); // 10�ʰ� �̴� ���
                currentSizeState = SizeState.Mini;
                uiManager.ShowBuff("mini", 15f);
                Destroy(collision.gameObject) ;
                break;
            case "Mini":
                if (sizeBuffCoroutine != null) StopCoroutine(sizeBuffCoroutine);
                sizeBuffCoroutine = StartCoroutine(MiniMode(10f)); // 10�ʰ� �̴� ���
                currentSizeState = SizeState.Mini;
                uiManager.ShowBuff("mini", 10f);
                break;
            case "superSpeed":
                moveSpeed += 55;
                Destroy(collision.gameObject);
                break;
        }
    }

    private IEnumerator GiantMode(float duration)
    {
        isGiant = true;
        currentSizeState = SizeState.Giant;

        yield return new WaitForSeconds(duration);

        isGiant = false;
        currentSizeState = SizeState.Normal;
        uiManager.HideBuff("giant");
    }


    private IEnumerator MiniMode(float duration)
    {
        isCmini = true;
        yield return new WaitForSeconds(duration);
        isCmini = false;
        uiManager.HideBuff("mini");
    }

    private IEnumerator TemporaryBoost(string buffName, float speed, float jump, float duration)
    {
        moveSpeed += speed;
        jumpForce += jump;

        yield return new WaitForSeconds(duration);

        moveSpeed -= speed;
        jumpForce -= jump;
        uiManager.HideBuff(buffName);
    }
}