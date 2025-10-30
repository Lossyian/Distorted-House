using System.Xml.Linq;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Door connectedDoor;
    public DoorTeleportTrigger teleportTrigger;

    private void Awake()
    {
        // 자식 중에 DoorTeleportTrigger가 이미 있으면 가져오기
        if (teleportTrigger == null)
            teleportTrigger = GetComponentInChildren<DoorTeleportTrigger>();

        // 없으면 새로 생성
        if (teleportTrigger == null)
        {
            GameObject triggerObj = new GameObject("DoorTrigger_" + name);
            triggerObj.transform.SetParent(transform);
            triggerObj.transform.localPosition = Vector3.zero;

            teleportTrigger = triggerObj.AddComponent<DoorTeleportTrigger>();

            BoxCollider2D col = triggerObj.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(0.8f, 0.8f); // 문 앞 감지 범위
        }

        // 자신 트리거의 door 필드 세팅 (나중에 역참조용)
        teleportTrigger.ownerDoor = this;
    }

    public void ConnectDoor(Door otherDoor)
    {
        // 문끼리 연결
        connectedDoor = otherDoor;
        otherDoor.connectedDoor = this;

        // 두 문 모두 트리거 생성/할당 완료 후 트리거끼리 연결
        if (teleportTrigger == null)
            teleportTrigger = GetComponentInChildren<DoorTeleportTrigger>();
        if (otherDoor.teleportTrigger == null)
            otherDoor.teleportTrigger = otherDoor.GetComponentInChildren<DoorTeleportTrigger>();

        if (teleportTrigger != null && otherDoor.teleportTrigger != null)
        {
            teleportTrigger.linkedTrigger = otherDoor.teleportTrigger;
            otherDoor.teleportTrigger.linkedTrigger = teleportTrigger;

            Debug.Log($"{name} ↔ {otherDoor.name} 트리거 연결 완료");
        }
        else
        {
            Debug.LogWarning($"{name} 또는 {otherDoor.name} 트리거 연결 실패 (트리거가 아직 없음)");
        }
    }
}