using UnityEngine;

public class RadarWaveEffect : MonoBehaviour
{
    [SerializeField] float expandSpeed = 10f;
    [SerializeField] float maxScale = 8f;
    [SerializeField] float fadeSpeed = 1.5f;

    private SpriteRenderer sprite;
    private Color baseColor;
    private bool expanding = false;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        baseColor = sprite.color;
        sprite.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0);
        transform.localScale = Vector3.zero;
    }

    public void StartWave()
    {
        StopAllCoroutines();
        StartCoroutine(ExpandWave());
    }

    private System.Collections.IEnumerator ExpandWave()
    {
        expanding = true;
        transform.localScale = Vector3.zero;
        Color c = baseColor;
        c.a = 0.4f;
        sprite.color = c;

        while (transform.localScale.x < maxScale)
        {
            transform.localScale += Vector3.one * expandSpeed * Time.deltaTime;
            yield return null;
        }

       
        float alpha = sprite.color.a;
        while (alpha > 0)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            c.a = alpha;
            sprite.color = c;
            yield return null;
        }

        sprite.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0);
        transform.localScale = Vector3.zero;
        expanding = false;
    }
}

