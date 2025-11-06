using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GhostChase : MonoBehaviour
{
    private Rigidbody2D rb;
    private Transform Player;

    [Header("이동 속도")]
    [SerializeField] private float chaseSpeed = 30.0f;
    [SerializeField] private float wanderSpeed = 15.0f;

    [Header("순찰 관련")]
    [SerializeField] private float wanderRadius = 8f;
    [SerializeField] private float changeTargetInterval = 3f;

    private Vector2 wanderTarget;
    private bool isWandering = false;
    private float lastTargetChangeTime = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        var playerObj = FindObjectOfType<PlayerController>();
        if (playerObj != null)
            Player = playerObj.transform;

        PickNewWanderTarget();
    }

    private void FixedUpdate()
    {
        if (isWandering)
        {
            Wander();
        }
        else
        {
            ChasePlayer();
        }
    }

    private void ChasePlayer()
    {
        if (Player == null) return;

        float currentSpeed = chaseSpeed * GameManager.ghostSpeedMulitplier;
        Vector2 dir = (Player.position - transform.position).normalized;
        rb.MovePosition(rb.position + dir * currentSpeed * Time.fixedDeltaTime);
    }

    private void Wander()
    {
        float currentSpeed = wanderSpeed * GameManager.ghostSpeedMulitplier;

        // 일정 간격마다 랜덤 타겟 갱신
        if (Time.time - lastTargetChangeTime > changeTargetInterval ||
            Vector2.Distance(transform.position, wanderTarget) < 1f)
        {
            PickNewWanderTarget();
        }

        Vector2 dir = (wanderTarget - (Vector2)transform.position).normalized;
        rb.MovePosition(rb.position + dir * currentSpeed * Time.fixedDeltaTime);
    }

    private void PickNewWanderTarget()
    {
        wanderTarget = (Vector2)transform.position + Random.insideUnitCircle * wanderRadius;
        lastTargetChangeTime = Time.time;
    }

    public void SetWanderMode(bool wander)
    {
        isWandering = wander;
        if (wander)
        {
            PickNewWanderTarget();
            Debug.Log(" 유령: 순찰 모드로 전환!");
        }
        else
        {
            Debug.Log(" 유령: 플레이어 추격 모드로 전환!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isWandering && collision.CompareTag("Player"))
        {
            Debug.Log("아앗.. 잡혀버렸다요..");
            GameManager.Instance.OnPlayerCaught();
        }
    }
}