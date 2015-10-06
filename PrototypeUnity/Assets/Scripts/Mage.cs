using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Mage : MonoBehaviour
{
    public Mage opponent;

    // General
    public int player_number = 1;
    public Color player_color = Color.red;
    public bool ai = false;
    private PlayerController pc;
    private Rigidbody2D rb;
    public SpriteRenderer sprite;
    public Transform floating_point;

    // Spells and control groups
    private string[] group_names = new string[] { "A", "B" };
    private int active_group = 0;
    public System.Action<int> event_group_swtich;

    private List<Projectile>[] projectile_groups;
    public Projectile prefab_fireball, prefab_iceball, prefab_waterball, prefab_shieldbreaker;
    public Transform cast_point;

    // State
    private bool invincible = true;


    // PUBLIC MODIFIERS

    public void RemoveProjectile(Projectile p)
    {
        projectile_groups[p.GetGroupNumber()].Remove(p);
    }


    // PUBLIC ACCESSORS

    public int GetActiveGroupNum()
    {
        return active_group;
    }
    public List<Projectile> GetProjectiles()
    {
        return projectile_groups[0];
    }


    // PRIVATE MODIFIERS

    private void Start()
    {
        // Color
        sprite.color = player_color;

        // Player controller
        if (ai)
        {
            gameObject.AddComponent<AIPlayerController>();
            GetComponent<AIPlayerController>().Initialize(this, opponent);
        }
        else
        {
            gameObject.AddComponent<HumanPlayerController>();
            GetComponent<HumanPlayerController>().Initialize(player_number);
        }
        this.pc = GetComponent<PlayerController>();

        // input events
        pc.InputCast += OnCastSpell;
        pc.InputScrollGroups += OnScrollGroups;

        // other
        rb = GetComponent<Rigidbody2D>();
        Refresh();

        // spells
        projectile_groups = new List<Projectile>[group_names.Length];
        for (int i = 0; i < projectile_groups.Length; ++i)
            projectile_groups[i] = new List<Projectile>();

        // TEST SPELL CREATION
        InstantiateSpell("ybb", 0);
        InstantiateSpell("xxa", 0);
        InstantiateSpell("xyx", 0);
        InstantiateSpell("ybb", 0);
        InstantiateSpell("xxa", 0);
        InstantiateSpell("xyx", 0);
        //InstantiateSpell("xyx", 1);
        //InstantiateSpell("xxa", 1);
        //InstantiateSpell("xxa", 1);

    }
    private void Update()
    {
        foreach (Projectile p in projectile_groups[active_group])
        {
            p.UpdateMovement(pc.InputMove);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (invincible) return;
        if (collision.collider.CompareTag("Ball"))
        {
            TakeHit();
        }
        else
        {
            Projectile p = collision.collider.GetComponent<Projectile>();
            if (p != null && p.proj_type == ProjectileType.Curse)
            {
                TakeHit();
            }
        }

    }

    private void Refresh()
    {
        StopAllCoroutines();
        StartCoroutine(FloatUp());
    }
    private void TakeHit()
    {
        rb.gravityScale = 1;
        invincible = true;
        StopAllCoroutines();
        StartCoroutine(RefreshAfterWait());
    }

    private void OnScrollGroups(int scroll)
    {
        active_group = (int)Mathf.Repeat(active_group + scroll, projectile_groups.Length);
        if (event_group_swtich != null)
            event_group_swtich(active_group);
    }
    private void OnCastSpell()
    {
        InstantiateSpell(pc.InputSpellCode, active_group);
    }
    private void InstantiateSpell(string spell_code, int group)
    {
        Projectile p = null;
        Vector2 offset = GeneralHelpers.RandomDirection2D() * 0.5f;
        Vector2 pos = (Vector2)cast_point.position + offset;


        switch (spell_code)
        {
            case "ybb":
                p = Instantiate<Projectile>(prefab_fireball);
                p.Initialize(this, pos, group, group_names[group]);
                break;
            case "xyx":
                p = Instantiate<Projectile>(prefab_iceball);
                p.Initialize(this, pos, group, group_names[group]);
                break;
            case "xxa":
                p = Instantiate<Projectile>(prefab_waterball);
                p.Initialize(this, pos, group, group_names[group]);
                break;
            case "xxxbbb":
                p = Instantiate<Projectile>(prefab_shieldbreaker);
                p.Initialize(this, pos, group, group_names[group]);
                break;
        }

        if (p != null) projectile_groups[group].Add(p);
    }

    private IEnumerator RefreshAfterWait()
    {
        yield return new WaitForSeconds(4f);
        Refresh();
    }
    private IEnumerator FlashInvincible()
    {
        float start_time = Time.time;
        float duration = 2f;
        Color c;

        while (true)
        {
            c = sprite.color;
            c.a = 1 - c.a;
            sprite.color = c;

            yield return new WaitForSeconds(0.025f);

            if (Time.time - start_time >= duration) break;
        }

        c = sprite.color;
        c.a = 1;
        sprite.color = c;

        invincible = false;
    }
    private IEnumerator FloatUp()
    {
        float t = 0;

        rb.angularVelocity = 0;
        while (t < 1)
        {
            t += Time.deltaTime;
            rb.MoveRotation(Mathf.Lerp(rb.rotation, 0, t));
            yield return null;
        }
        t = 0;

        rb.gravityScale = 0;
        StartCoroutine(FlashInvincible());

        while (t < 1)
        {
            t += Time.deltaTime / 15f;
            rb.MovePosition(Vector2.Lerp(transform.position, floating_point.position, t));
            yield return null;
        }

        // insure final pos, rotation
        transform.position = floating_point.position;
        transform.rotation = Quaternion.identity;
    }


}
