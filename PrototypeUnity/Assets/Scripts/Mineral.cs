using UnityEngine;
using System.Collections;

public class Mineral : MonoBehaviour
{
    public Color[] colors;

    public const float WidthHeight = 0.2828f;



    private void Start()
    {
        GetComponent<SpriteRenderer>().color = colors[Random.Range(0, colors.Length)];
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Projectile"))
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0.5f;
            GetComponent<Collider2D>().enabled = false;

            StartCoroutine(Shrink());
        }
    }

    private IEnumerator Shrink()
    {
        Vector3 scale_original = transform.localScale;

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(scale_original, Vector3.zero, t*t);
            yield return null;
        }

        Destroy(gameObject);
    }
}
