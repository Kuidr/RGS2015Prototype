using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MineralSpawner : MonoBehaviour
{
    public Mineral mineral_prefab;
    private Mineral root_mineral = null;

    private void Start()
    {
        StartCoroutine(UpdateSpawning());
        GetComponent<SpriteRenderer>().enabled = false;
    }

    private IEnumerator UpdateSpawning()
    {
        while (true)
        {
            // grow minerals
            for (int i = 0; i < Random.Range(5, 10); ++i)
            {
                SpawnMineral();
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(Random.Range(30f, 40f));
        }
    }

    private void SpawnMineral()
    {
        Mineral m = Instantiate<Mineral>(mineral_prefab);

        if (root_mineral == null)
        {
            m.transform.position = transform.position;
            root_mineral = m;
        }
        else
        {
            Stack<Mineral> parent_chain = new Stack<Mineral>();
            parent_chain.Push(root_mineral);
            PlaceMineral(m, parent_chain);
        }
    }
    private bool PlaceMineral(Mineral m, Stack<Mineral> parent_chain)
    {
        // Check if there is a free spot or another mineral in each horizontal direction
        Vector2[] dir_list = new Vector2[] { Vector2.right, -Vector2.right, Vector2.up, -Vector2.up };
        dir_list = GeneralHelpers.ShuffleArray(dir_list);

        bool prefer_chaining = true;


        for (int i = 0; i < 4; ++i)
        {
            Vector2 point = (Vector2)parent_chain.Peek().transform.position + dir_list[i] * Mineral.WidthHeight;
            Collider2D col = Physics2D.OverlapPoint(point);
            if (col == null)
            {
                if (prefer_chaining) continue; // try another direction to find a mineral to chain onto
                else
                {
                    // place mineral here (base case)
                    m.transform.position = point;
                    return true;
                }
            }
            
            // is there another mineral in the way? (that was not previously a parent in this recursion)
            Mineral new_parent = col.GetComponent<Mineral>();
            if (new_parent != null && !parent_chain.Contains(new_parent))
            {
                // try to place mineral further down the chain
                Stack<Mineral> new_chain = new Stack<Mineral>(parent_chain);
                new_chain.Push(new_parent);
                if (PlaceMineral(m, new_chain)) return true; 
            }
        }

        // we tried to find a spot further down the chain, but couldn't find one
        // now look at the immediatly open spots again
        if (prefer_chaining)
        {
            for (int i = 0; i < 4; ++i)
            {
                Vector2 point = (Vector2)parent_chain.Peek().transform.position + dir_list[i] * Mineral.WidthHeight;
                Collider2D col = Physics2D.OverlapPoint(point);
                if (col == null)
                {
                    // place mineral here
                    m.transform.position = point;
                    return true;
                }
            }
        }

        // no free locations, and we are as far down the chain as possible (unusual)
        return false;
    }
}
