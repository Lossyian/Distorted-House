using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class ScanVisualizer : MonoBehaviour
{
    private LineRenderer line;
    private int segments = 100;
    private float currentRadius = 0f;

    [SerializeField] Color scanColor = new Color(0.3f, 1f, 1f, 0.4f);
    [SerializeField] float lineWidth = 0.05f;


    private void Awake()
    {
       line = GetComponent<LineRenderer>();
        line.loop = true; 
        line.positionCount = segments;
        line.startColor = scanColor;
        line.endColor = scanColor;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.enabled = false;
    }

    public void BeginScan()
    {
        line.enabled=true; 
    }

    public void updateRadius(float radius)
    {
        currentRadius = radius;
        DrawCircle(radius);

    }

    private void DrawCircle(float radius)
    {
        for (int i = 0; i< segments; i++)
        {
            float angle = 2 * Mathf.PI * i / segments;
            Vector3 pos = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);
            line.SetPosition(i, pos);
        }
    }

    public void EndScan()
    {
        line.enabled=false;
    }
}
