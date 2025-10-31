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
    {   //현 스크립트가 붙은 게임 오브젝트에 있는 콜라이더 탐색
        var col = GetComponent<BoxCollider2D>();
        if (col == null)  //콜라이더2D 오브젝트가 없다면,새로 만든다.
            col = gameObject.AddComponent<BoxCollider2D>();
        //해당 콜라이더에 트리거를 체크해 설정한다.
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (linkedTrigger == null || isOnCooldown) return;
        if (!other.CompareTag("Player")) return;
        // 쿨타임이 차있고,, 텔레포트 지점이 연결되어있고, 태그가 플레이어라면 텔레포트 실행.
        StartCoroutine(TeleportWithCooldown(other));
    }

    private IEnumerator TeleportWithCooldown(Collider2D player)
    {
        // 쿨타임동안 작동 중지
        isOnCooldown = true;

        //  반대쪽 콜라이더 트리거도 비활성화
        if (linkedTrigger != null)
            linkedTrigger.isOnCooldown = true;

        //  반대쪽 콜라이더 트리거의 position으로 플레이어 텔레포트
        Vector3 targetPos = linkedTrigger.transform.position;
        player.transform.position = targetPos + Vector3.up * 0.5f;

        //  반대쪽 콜라이더 위치로 TP함으로, 무한히 계속 TP가 되는것의 방지를 위해 코루틴을 이용해 TP 쿨타임을 적용.
        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false;
        if (linkedTrigger != null)
            linkedTrigger.isOnCooldown = false;
    }
}