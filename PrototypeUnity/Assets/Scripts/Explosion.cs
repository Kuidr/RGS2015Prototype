using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour
{
    private ParticleSystem ps;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        gameObject.SetActive(false);
    }

	public void Explode(float radius, float max_force)
    {
        gameObject.SetActive(true);
        ps.Clear();
        ps.Play();

        // Apply force in radius (exponentially decreasing with distance)
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D col in cols)
        {
            Rigidbody2D r = col.GetComponent<Rigidbody2D>();
            if (r != null && col.CompareTag("Projectile") || col.CompareTag("Ball"))
            {
                Vector2 v = r.position - (Vector2)transform.position;
                float force = (1 - Mathf.Pow(v.magnitude / radius, 2f)) * max_force;
                r.AddForceAtPosition(v.normalized * force, transform.position, ForceMode2D.Impulse);
            }
        }
    }
    public void Update()
    {
        if (!ps.isPlaying)
        {
            ps.Clear();
            gameObject.SetActive(false);
        }
    }

    public bool IsExploding()
    {
        return ps.isPlaying;
    }

}
