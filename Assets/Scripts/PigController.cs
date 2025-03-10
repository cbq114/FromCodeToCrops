using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class PigController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float changeTargetTime = 3f;
    public float moveRadius = 3f;

    private Vector2 targetPosition;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private AudioSource audioSource;

    public AudioClip walkSound; // Âm thanh di chuyển


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>(); 
        StartCoroutine(ChangeTargetRoutine());
        rb.freezeRotation = true;
        audioSource = GetComponent<AudioSource>();
        AudioManager.instance.sfx = AudioManager.instance.sfx.Append(audioSource).ToArray();
    }

    void Update()
    {
        MoveToTarget();
    }

    void MoveToTarget()
    {
        float distance = Vector2.Distance(transform.position, targetPosition);

        if (distance > 0.2f) 
        {
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;

            
            if (Mathf.Abs(direction.x) > 0.1f)
            {
                spriteRenderer.flipX = direction.x > 0;
            }

            animator.SetBool("isWalking", true);
            if (!audioSource.isPlaying)
            {
                audioSource.clip = walkSound;
                audioSource.volume = 3.0f;
                audioSource.Play();
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isWalking", false);
        }
    }

    IEnumerator ChangeTargetRoutine()
    {
        while (true)
        {
            ChangeTargetPosition(); 
            animator.SetBool("isWalking", true); 

            yield return new WaitForSeconds(changeTargetTime);

            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isWalking", false); 

            yield return new WaitForSeconds(4f);
        }
    }

    void ChangeTargetPosition()
    {
        Vector2 newTarget;
        do
        {
            float x = transform.position.x + Random.Range(-moveRadius, moveRadius);
            float y = transform.position.y + Random.Range(-moveRadius, moveRadius);
            newTarget = new Vector2(x, y);
        } while (Vector2.Distance(newTarget, transform.position) < 0.5f);

        targetPosition = newTarget;
    }
}
