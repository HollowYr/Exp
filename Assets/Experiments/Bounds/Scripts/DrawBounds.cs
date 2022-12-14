using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawBounds : MonoBehaviour
{
    public Color color_sphere = new Color(0.0f, 0.0f, 0.0f, 0.5f);
    public Color color_bounds = new Color(1.0f, 1.0f, 0.0f, 0.5f);
    public bool Hierarchical = false;

    public void OnDrawGizmos()
    {
        Bounds b = new Bounds();
        if (Hierarchical)
        {
            Renderer[] r = gameObject.GetComponentsInChildren<Renderer>();
            b = ComputeBounds(r);
        }
        else
        {
            Renderer r = gameObject.GetComponent<Renderer>();
            if (r != null)
            {
                b = r.bounds;
            }
        }
        Gizmos.color = color_bounds;
        Gizmos.DrawCube(b.center, b.size);
        Gizmos.color = color_sphere;
        Gizmos.DrawSphere(b.center, b.size.magnitude * 0.1f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(b.max, .1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(b.min, .1f);

    }


    public Bounds ComputeBounds(Renderer[] renderers)
    {
        Bounds bounds = new Bounds();
        for (int ir = 0; ir < renderers.Length; ir++)
        {
            Renderer renderer = renderers[ir];
            if (ir == 0)
                bounds = renderer.bounds;
            else
                bounds.Encapsulate(renderer.bounds);
        }
        return bounds;
    }
}
