using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Room : MonoBehaviour
{
    public List<Door> doors;
    public List<InvestigatePoint> points = new List<InvestigatePoint>();
   

    private void Awake()
    {
        points.AddRange(GetComponentsInChildren<InvestigatePoint>());
    }

    

    public Door GetRandomDoor(Door ex = null)
    {    // 룸을 상속받은 자식오브젝트 수 중 랜덤으로 하나 선정)
        List<Door> available = new List<Door>(doors);
        if (ex != null)
        {
            available.Remove(ex);
        }
        if (available.Count == 0)
        {
            return null;
        }
        return available[Random.Range(0, available.Count)];
    }

    public void LockrandomDoor()
    {
        if (doors == null || doors.Count == 0) return;

        Door target = doors[Random.Range(0, doors.Count)];
        Collider2D col = target.GetComponent<Collider2D>();
        if (col != null )
        {
            col.enabled = false;
            Debug.Log("문이 잠겼다요!");
        }
    }
    private void Reset()
    {
        // 자동으로 트리거 설정
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, GetComponent<Collider2D>().bounds.size);
    }


}
