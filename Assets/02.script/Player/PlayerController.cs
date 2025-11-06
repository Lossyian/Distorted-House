using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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

    public bool isHiding { get;  set; } = false;
    private SpriteRenderer sprite;

    private float currentScanRadius = 0f;
    private float nextScanTime = 0f;
    private bool isScanning = false;

    private ScanVisualizer Visualizer;
    public Room currentRoom;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        Visualizer = GetComponentInChildren<ScanVisualizer>();
        sprite = GetComponent<SpriteRenderer>();
    }
    

    
    void Update()
    {
        if (isHiding)
        {
            rigid.velocity = Vector2.zero; 
            return;
        }

        hor = Input.GetAxis("Horizontal");
        ver = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(interactKey))
        {
            
            TryInteract();
            GetComponent<NoiseEmitter>()?.OnInvestigate();
        }

        if(Input.GetKeyDown(Scankey)&& Time.time >= nextScanTime && !isScanning)
        {
            StartCoroutine(PerformScanWave());
            GetComponent<NoiseEmitter>()?.OnScan();
        }
    }

    private void FixedUpdate()
    {
        PlayerMove();
    }
    
    void PlayerMove()
    {
        if (isHiding)
        {
            rigid.velocity = Vector2.zero;
            return;
        }
        Vector2 Velo = new Vector2(hor , ver).normalized;
        rigid.velocity = Vector2.Lerp(rigid.velocity, Velo* moveSpeed, Time.deltaTime * moveSpeed);
    }
    private void TryInteract()
    {
        Collider2D hit = Physics2D.OverlapCircle(interactiveHart.position, interactRange, interactableLayer);

        if (hit != null)
        {
            Debug.Log("내안에 있다요");
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

    public IEnumerator HideInPoint(InvestigatePoint point)
    {
        if (!NoiseSystem.Instance.isHunting)
        {
            UiManager.instance?.ShowDialog("숨을만한 곳이다 여차하면 여기 숨으면...");
            yield break;
        }

        if (isHiding)
        {
            UiManager.instance?.ShowDialog("이미 숨어있다.");
            yield break;
        }

        isHiding = true;
        sprite.enabled = false;
        rigid.velocity = Vector2.zero;

        UiManager.instance.ShowDialog("숨을만한곳이다. 여기에숨자!");
        GhostManager.Instance?.SetGhostToWanderMode(true);

        yield return new WaitForSeconds(5f);

        sprite.enabled = true;
        isHiding = false;
        UiManager.instance?.ShowDialog("여기 더 숨을수 없겠어..!");

        GhostManager.Instance?.SetGhostToWanderMode(false);
        point.MarkAsUsedHideSpot();
    }

    
    private void OnDrawGizmosSelected()
    {
        if (interactiveHart != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(interactiveHart.position, interactRange);
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Room room = other.GetComponent<Room>();
        if (room != null)
        {
            currentRoom = room;
            Debug.Log($"{room.name} 방에 들어왔다요");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Room room = other.GetComponent<Room>();
        if (room != null && currentRoom == room)
        {
            Debug.Log($" {room.name} 방에서 나갔다요");
            currentRoom = null;
        }
    }
}
