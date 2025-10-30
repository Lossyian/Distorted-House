using UnityEngine;

using System.Collections;

public class DoorTeleportTrigger : MonoBehaviour
{
    [Header("도킹된 반대쪽 트리거")]
    public DoorTeleportTrigger linkedTrigger;

    [HideInInspector] public Door ownerDoor;

    [Header("텔레포트 쿨타임")]
    [SerializeField] private float cooldownTime = 0.5f;

    private bool isOnCooldown = false;

    private void Awake()
    {
        var col = GetComponent<BoxCollider2D>();
        if (col == null)
            col = gameObject.AddComponent<BoxCollider2D>();

        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (linkedTrigger == null || isOnCooldown) return;
        if (!other.CompareTag("Player")) return;

        StartCoroutine(TeleportWithCooldown(other));
    }

    private IEnumerator TeleportWithCooldown(Collider2D player)
    {
        // 쿨타임 시작
        isOnCooldown = true;

        //  상대 트리거도 잠깐 비활성화 (양쪽 루프 방지)
        if (linkedTrigger != null)
            linkedTrigger.isOnCooldown = true;

        //  플레이어 이동
        Vector3 targetPos = linkedTrigger.transform.position;
        player.transform.position = targetPos + Vector3.up * 0.5f;

        //  대기 후 쿨타임 해제
        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false;
        if (linkedTrigger != null)
            linkedTrigger.isOnCooldown = false;
    }
}