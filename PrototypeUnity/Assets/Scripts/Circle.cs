using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class Circle : MonoBehaviour
{
    private LineRenderer line;
    public float resolution = 2;

    private float radius = 2;
    private Vector2 position = Vector2.zero;
    
    

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        position = transform.position;
        Set(radius, Color.white, 0.05f);
    }
    private void Draw()
    {
        float circumference = Mathf.PI * radius * 2f;
        int n = (int)(circumference * resolution);
        float angle_inc = (Mathf.PI*2f) / (n-1);
        line.SetVertexCount(n);


        for (int i = 0; i < n; ++i)
        {
            float a = angle_inc * i;
            line.SetPosition(i, position + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * radius);
        }
    }

    public void Set(Vector2 pos, float r, Color color, float line_width)
    {
        position = pos;
        radius = r;
        line.SetColors(color, color);
        line.SetWidth(line_width, line_width);
        Draw();
    }
    public void Set(float r, Color color, float line_width)
    {
        radius = r;
        line.SetColors(color, color);
        line.SetWidth(line_width, line_width);
        Draw();
    }
    public void SetPosition(Vector2 pos)
    {
        position = pos;
        Draw();
    }

    public Vector2 GetPosition()
    {
        return position;
    }
    public float GetRadius()
    {
        return radius;
    }
    public Vector2 RandomPointOnCircle()
    {
        float a = Random.value * Mathf.PI*2f;
        return position + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * radius;
    }
}
