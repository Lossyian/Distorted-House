using System.Xml.Linq;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Door connectedDoor;
    public DoorTeleportTrigger teleportTrigger;

    private void Awake()
    {
        // 자식 중에 DoorTeleportTrigger를 가진 오브젝트가 이미 있으면 호출하기
        if (teleportTrigger == null)
            teleportTrigger = GetComponentInChildren<DoorTeleportTrigger>();

        // 없으면 새로 생성하기.
        if (teleportTrigger == null)
        {
            GameObject triggerObj = new GameObject("DoorTrigger_" + name);
            triggerObj.transform.SetParent(transform);
            triggerObj.transform.localPosition = Vector3.zero;

            teleportTrigger = triggerObj.AddComponent<DoorTeleportTrigger>();

            BoxCollider2D col = triggerObj.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(1.0f, 1.0f); // 문 앞 감지 범위
        }

        // 자신 트리거의 door 필드 세팅 
        teleportTrigger.ownerDoor = this;
    }

    public void ConnectDoor(Door otherDoor)
    {
        // 문끼리 서로 연결하기 문 안의 스크립트 변수에 서로를 할당.
        connectedDoor = otherDoor;
        otherDoor.connectedDoor = this;

        // 두 문 모두 트리거 생성 완료 후 트리거끼리 연결
        if (teleportTrigger == null)
        { //트리거변수가 비어있다면, 자식 오브젝트중, DoorTeleportTrigger 스크립트를 가진 오브젝트를 트리거 변수에 할당.
            teleportTrigger = GetComponentInChildren<DoorTeleportTrigger>();
        }
        if (otherDoor.teleportTrigger == null)
            otherDoor.teleportTrigger = otherDoor.GetComponentInChildren<DoorTeleportTrigger>();

        if (teleportTrigger != null && otherDoor.teleportTrigger != null)
        { // 현재 문에 있는 트리거의 linkedTrigger에다, 반대쪽 문의 트리거를 할당
            teleportTrigger.linkedTrigger = otherDoor.teleportTrigger;
            //반대쪽 문도 똑같이 적용.
            otherDoor.teleportTrigger.linkedTrigger = teleportTrigger;

            Debug.Log($"{name} ↔ {otherDoor.name} 트리거 연결 완료");
        }
        else
        {
            Debug.LogWarning($"{name} 또는 {otherDoor.name} 트리거 연결 실패 (트리거가 아직 없음)");
        }
    }
}