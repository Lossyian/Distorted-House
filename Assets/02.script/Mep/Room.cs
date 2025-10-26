using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public List<Door> doors;

    public Door GetRandomDoor()
    {    // 룸을 상속받은 자식오브젝트 수 중 랜덤으로 하나 선정)
        return doors[Random.Range(0, doors.Count)];
    }
}
