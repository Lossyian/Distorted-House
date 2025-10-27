using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Room : MonoBehaviour
{
    public List<Door> doors;
    public List<InvestigatePoint> points = new List<InvestigatePoint>();

    private void Awake()
    {
        points.AddRange(GetComponentsInChildren<InvestigatePoint>());
    }
    public Door GetRandomDoor()
    {    // 룸을 상속받은 자식오브젝트 수 중 랜덤으로 하나 선정)
        return doors[Random.Range(0, doors.Count)];
    }
}
