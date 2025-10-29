using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("상호작용")]
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] float interactRange = 2.0f;
    [SerializeField] Transform interactiveHart;
    [SerializeField] KeyCode interactKey = KeyCode.F;

    [Header("이동")]
    private Rigidbody2D rigid;
    [SerializeField] float moveSpeed = 20.0f;
    private float hor, ver;

    [Header("둘러보기")]
    [SerializeField] KeyCode Scankey = KeyCode.Space;
    [SerializeField] float scanMaxRadius = 8f;
    [SerializeField] float scanSpeed = 10f;
    [SerializeField] float scanCooldown = 1.5f;

    private float currentScanRadius = 0f;
    private float nextScanTime = 0f;
    private bool isScanning = false;

    private ScanVisualizer Visualizer;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        Visualizer = GetComponentInChildren<ScanVisualizer>();
    }
    

    
    void Update()
    {
        hor = Input.GetAxis("Horizontal");
        ver = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(interactKey))
        {
            Debug.Log("F키다욧");
            TryInteract();
        }

        if(Input.GetKeyDown(Scankey)&& Time.time >= nextScanTime && !isScanning)
        {
            StartCoroutine(PerformScanWave());
        }
    }

    private void FixedUpdate()
    {
        PlayerMove();
    }
    
    void PlayerMove()
    {
        Vector2 Velo = new Vector2(hor * moveSpeed, ver * moveSpeed);
        rigid.velocity = Vector2.Lerp(rigid.velocity, Velo, Time.deltaTime * moveSpeed);
    }
    private void TryInteract()
    {
        Collider2D hit = Physics2D.OverlapCircle(interactiveHart.position, interactRange, interactableLayer);

        if (hit != null)
        {
            Debug.Log("내안에 들어왔다요");
            InvestigatePoint point = hit.GetComponent<InvestigatePoint>();
            if (point != null)
            {
                point.Interact();
            }

        }
    }

    private IEnumerator PerformScanWave()
    {
        GetComponentInChildren<RadarWaveEffect>()?.StartWave();
        isScanning = true;
        currentScanRadius = 0f;
        nextScanTime = Time.time+scanCooldown;

        if (Visualizer != null)
        {
            Visualizer.BeginScan();
        }

        while (currentScanRadius <scanMaxRadius)
        {
            currentScanRadius += Time.deltaTime * scanSpeed;

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, currentScanRadius, interactableLayer);

            foreach (var hit in hits)
            {
                var point = hit.GetComponent<InvestigatePoint>();
                if (point !=null)
                {
                    float dist = Vector2.Distance(transform.position, point.transform.position);
                    point.SetVisibleByWave(dist, currentScanRadius);
                }
            }
            if (Visualizer != null)
            {
                Visualizer.updateRadius(currentScanRadius);

            }
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        if (Visualizer != null)
        {
            Visualizer.EndScan();
        }

        foreach(var point in FindObjectsOfType<InvestigatePoint>())
        {
            point.SetVisible(false);
        }
        isScanning = false;

       

    }


    private void OnDrawGizmosSelected()
    {
        if (interactiveHart != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(interactiveHart.position, interactRange);
        }

    }
}
