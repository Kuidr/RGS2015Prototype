using UnityEngine;
using System.Collections;

public enum ProjectileType { Fire, Ice, Water, Curse }

public class Projectile : MonoBehaviour
{
    public ProjectileType proj_type;
    private Mage caster;
    private int group_num = 0;

    // movement
	private Rigidbody2D rb;
    private float steering_force_factor = 1;
    private float max_steering_force = 3;
    private float max_speed = 3;

	// visual
    private ParticleSystem ps;
	public TextMesh marker;
	
    // impact
    public Explosion explosion_obj;
    private bool exploded = false;



    // PUBLIC MODIFIERS

    public void Initialize(Mage caster, Vector2 pos, int group_num, string group_name)
    {
        this.caster = caster;
        this.group_num = group_num;
        marker.color = caster.player_color;
        transform.position = pos;

        caster.event_group_swtich += OnCasterGroupSwitch;

        marker.text = group_name;
        OnCasterGroupSwitch(caster.GetActiveGroupNum());
    }
    public void UpdateMovement(Vector2 input_move)
    {
        if (exploded) return;

        Vector2 direction = new Vector2(input_move.x, input_move.y);
        if (direction.magnitude > 0.3f)
        {
            Vector2 desired_velocity = direction * max_speed;

            // Steering force
            Vector2 steering_force = Clip(desired_velocity - rb.velocity, max_steering_force * steering_force_factor);
            rb.AddForce(steering_force);
        }

        // Clip speed
        rb.velocity = Clip(rb.velocity, max_speed);
    }


    // PUBLIC ACCESSORS

    public int GetGroupNumber()
    {
        return group_num;
    }


    // PRIVATE MODIFIERS

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
		ps = GetComponent<ParticleSystem>();
    }
    private void Update()
    {
        // Clip speed
        rb.velocity = Clip(rb.velocity, max_speed);

        if (exploded)
        {
            if (!explosion_obj.IsExploding()) UnExplode();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ball"))
        {
            Explode();   
        }
        else if (collision.collider.CompareTag("Projectile"))
        {
            Projectile p = collision.collider.GetComponent<Projectile>();
            if (p != null)
            {
                if (p.proj_type == ProjectileType.Fire && proj_type == ProjectileType.Ice ||
                    p.proj_type == ProjectileType.Ice && proj_type == ProjectileType.Water ||
                    p.proj_type == ProjectileType.Water && proj_type == ProjectileType.Fire ||
                    proj_type == ProjectileType.Curse)
                {
                    if (p.caster != caster)
                    {
                        this.Destroy();
                    }
                }
            }
        }
        else if (this.proj_type == ProjectileType.Curse)
        {
            Mage mage = collision.collider.GetComponent<Mage>();
            if (mage != null)
            {
                Vector3 direction = (mage.transform.position - transform.position).normalized;
                mage.GetComponent<Rigidbody2D>().AddForceAtPosition(direction * 40f, collision.contacts[0].point, ForceMode2D.Impulse);
                this.Destroy();
            }
        }
        else
        {
            StartCoroutine(WeakenSteeringForce());
        }
    }

    private void OnCasterGroupSwitch(int active_group)
    {
        if (active_group == group_num)
        {
            marker.fontSize = 80;
        }
        else
        {
            marker.fontSize = 50;
        }
    }

    private void Destroy()
    {
        caster.event_group_swtich -= OnCasterGroupSwitch;
        caster.RemoveProjectile(this);
        Destroy(gameObject);
    }

    private void Explode()
    {
        explosion_obj.Explode(2, 5);
        rb.velocity = Vector2.zero;
        transform.GetComponent<Collider2D>().enabled = false;

        // particles
        ps.Clear();
        ps.enableEmission = false;

        exploded = true;
    }
    private void UnExplode()
    {
        rb.isKinematic = false;
        rb.velocity = RandomDirection() * max_speed / 2f;
        transform.GetComponent<Collider2D>().enabled = true;

        ps.Play();
        ps.enableEmission = true;

        exploded = false;
    }

    private IEnumerator WeakenSteeringForce()
    {
        float t = 0;
        while (steering_force_factor < 1)
        {
            t += Time.deltaTime / 5f;
            steering_force_factor = Mathf.Pow(t, 2f);
            yield return null;
        }
        steering_force_factor = 1;
    }


    // PRIVATE ACCESSORS AND HELPERS

    private Vector2 Clip(Vector2 v, float max_magnitude)
    {
        if (v.magnitude > max_magnitude)
            return v.normalized * max_magnitude;
        return v;
    }
    private float Clip(float x, float max_magnitude)
    {
        if (Mathf.Abs(x) > max_magnitude)
            return Mathf.Sign(x) * max_magnitude;
        return x;
    }

    private Vector2 RandomDirection()
    {
        float angle = Random.Range(0, Mathf.PI*2f);
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }

}
